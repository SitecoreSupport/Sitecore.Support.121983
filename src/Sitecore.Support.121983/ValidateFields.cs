namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.FieldsValidation
{
  using Data.Fields;
  using Data.Items;
  using Data.Validators;
  using Diagnostics;
  using Sitecore.ExperienceEditor.Speak.Ribbon.Requests.FieldsValidation;
  using Sitecore.ExperienceEditor.Speak.Server.Contexts;
  using Sitecore.ExperienceEditor.Speak.Server.Requests;
  using Sitecore.ExperienceEditor.Speak.Server.Responses;
  using Sitecore.ExperienceEditor.Switchers;
  using Sitecore.ExperienceEditor.Utils;
  using System.Collections.Generic;
  using Server.Contexts;

  public class ValidateFields : PipelineProcessorRequest<PageContext>
  {
    public override PipelineProcessorResponseValue ProcessRequest()
    {
      Item item = base.RequestContext.Item;
      Assert.IsNotNull(item, "Item is null");
      using (new ClientDatabaseSwitcher(item.Database))
      {
        ValidatorCollection validators = ValidatorManager.GetFieldsValidators(ValidatorsMode.ValidatorBar, ValidationContext.GetControlsToValidate(base.RequestContext).Keys, item.Database);
        ValidatorManager.Validate(validators, new ValidatorOptions(true));
        List<FieldValidationError> list = new List<FieldValidationError>();
        foreach (BaseValidator validator in validators)
        {
          if (!validator.IsValid && !validator.FieldID.IsNull)
          {
            if (WebUtility.IsEditAllVersionsTicked())
            {
              Field field = item.Fields[validator.FieldID];
              if (!field.Shared && !field.Unversioned)
              {
                continue;
              }
            }
            FieldValidationError error = new FieldValidationError
            {
              Text = validator.Text,
              Title = validator.Name,
              FieldId = validator.FieldID.ToString(),
              DataSourceId = validator.ItemUri.ItemID.ToString(),
              Errors = validator.Errors,
              Priority = (int)validator.Result
            };
            list.Add(error);
          }
        }
        return new PipelineProcessorResponseValue { Value = list };
      }
    }
  }
}
