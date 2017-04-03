namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.SaveItem
{
  using Collections;
  using Data;
  using Sitecore.ExperienceEditor.Utils;
  using System.Collections.Generic;
  using Data.Validators;
  using Data.Fields;
  using Data.Items;
  using Diagnostics;
  using Sitecore.ExperienceEditor.Speak.Server.Contexts;
  using Globalization;
  using Shell.Applications.WebEdit.Commands;
  using Server.Contexts;

  public class Validators : Sitecore.ExperienceEditor.Speak.Ribbon.Requests.SaveItem.Validators
  {
    protected override IEnumerable<BaseValidator> GetValidators(Sitecore.Data.Items.Item item)
    {
      ValidatorsMode mode;
      SafeDictionary<FieldDescriptor, string> controlsToValidate = ValidationContext.GetControlsToValidate(base.RequestContext);
      ValidatorCollection validators = PipelineUtil.GetValidators(item, controlsToValidate, out mode);
      validators.Key = base.RequestContext.ValidatorsKey;
      return validators;

    }
  }
}