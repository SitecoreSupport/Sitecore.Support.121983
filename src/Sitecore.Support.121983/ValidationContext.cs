namespace Sitecore.Support.ExperienceEditor.Speak.Server.Contexts
{
  using System.Collections.Generic;
  using Collections;
  using Data;
  using Data.Fields;
  using Data.Items;
  using Data.Validators;
  using Diagnostics;
  using Sitecore.ExperienceEditor.Speak.Server.Contexts;
  using Sitecore.ExperienceEditor.Utils;
  using Globalization;
  using Shell.Applications.WebEdit.Commands;

  public static class ValidationContext
  {
    public static SafeDictionary<FieldDescriptor, string> GetControlsToValidate(PageContext context)
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
  }
}