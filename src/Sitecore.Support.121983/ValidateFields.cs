namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.FieldsValidation
{
  using Sitecore.Collections;
  using Sitecore.Data;
  using Sitecore.Data.Fields;
  using Sitecore.Data.Items;
  using Sitecore.Data.Validators;
  using Sitecore.Diagnostics;
  using Sitecore.ExperienceEditor.Speak.Ribbon.Requests.FieldsValidation;
  using Sitecore.ExperienceEditor.Speak.Server.Contexts;
  using Sitecore.ExperienceEditor.Speak.Server.Requests;
  using Sitecore.ExperienceEditor.Speak.Server.Responses;
  using Sitecore.ExperienceEditor.Switchers;
  using Sitecore.ExperienceEditor.Utils;
  using Sitecore.Globalization;
  using Sitecore.Shell.Applications.WebEdit.Commands;
  using System.Collections.Generic;

  public class ValidateFields : PipelineProcessorRequest<PageContext>
  {
    public SafeDictionary<FieldDescriptor, string> GetControlsToValidate(PageContext context)
    {
      Item item = context.Item;
      Assert.IsNotNull(item, "The item is null.");
      Language language = item.Language;
      IEnumerable<PageEditorField> fields = WebUtility.GetFields(item.Database, context.FieldValues);
      SafeDictionary<FieldDescriptor, string> dictionary = new SafeDictionary<FieldDescriptor, string>();
      foreach (PageEditorField field in fields)
      {
        Item item2 = (item.ID == field.ItemID) ? item : item.Database.GetItem(field.ItemID, language);
        Field field2 = item.Fields[field.FieldID];
        string str = WebUtility.HandleFieldValue(field.Value, field2.TypeKey);
        FieldDescriptor descriptor = new FieldDescriptor(item2.Uri, field2.ID, str, false);
        string str2 = field.ControlId ?? string.Empty;
        dictionary[descriptor] = str2;
        if (!string.IsNullOrEmpty(str2))
        {
          RuntimeValidationValues.Current[str2] = str;
        }
      }
      return dictionary;
    }

    public override PipelineProcessorResponseValue ProcessRequest()
    {
      Item item = base.RequestContext.Item;
      Assert.IsNotNull(item, "Item is null");
      using (new ClientDatabaseSwitcher(item.Database))
      {
        ValidatorCollection validators = ValidatorManager.GetFieldsValidators(ValidatorsMode.ValidatorBar, this.GetControlsToValidate(base.RequestContext).Keys, item.Database);
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
