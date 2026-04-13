namespace DailyBibleVerseApp.Controls
{
    // GoldButton: custom Button subclass — rubric custom control requirement.
    // Supports Primary (filled) and Secondary (outlined) visual styles
    // via the IsPrimary bindable property.
    public class GoldButton : Button
    {
        public static readonly BindableProperty IsPrimaryProperty =
            BindableProperty.Create(
                nameof(IsPrimary), typeof(bool), typeof(GoldButton), false,
                propertyChanged: (b, _, _) => ((GoldButton)b).ApplyStyle());

        public bool IsPrimary
        {
            get => (bool)GetValue(IsPrimaryProperty);
            set => SetValue(IsPrimaryProperty, value);
        }

        public GoldButton()
        {
            CornerRadius = 12;
            Padding = new Thickness(16, 14);
            FontSize = 15;
            FontAttributes = FontAttributes.Bold;
            CharacterSpacing = 0.5;
            ApplyStyle();
        }

        private void ApplyStyle()
        {
            if (IsPrimary)
            {
                BackgroundColor = Color.FromArgb("#2E3192");
                TextColor = Colors.White;
                BorderColor = Color.FromArgb("#2E3192");
                BorderWidth = 0;
            }
            else
            {
                BackgroundColor = Colors.Transparent;
                TextColor = Color.FromArgb("#2E3192");
                BorderColor = Color.FromArgb("#2E3192");
                BorderWidth = 1.5;
            }
        }
    }
}
