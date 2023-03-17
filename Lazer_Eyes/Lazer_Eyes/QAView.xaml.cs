using System;

namespace Lazer_Eyes;

public partial class QAView : Grid
{

    public static readonly BindableProperty AnswerTextProperty =
        BindableProperty.Create("AnswerText", typeof(string), typeof(QAView), "");

    public static readonly BindableProperty QuestionTextProperty =
        BindableProperty.Create("QuestionText", typeof(string), typeof(QAView), "");

    public string AnswerText
    {
        get => (string)GetValue(AnswerTextProperty);
        set => SetValue(AnswerTextProperty, value);
    }

    public string QuestionText
    {
        get => (string)GetValue(QuestionTextProperty);
        set => SetValue(QuestionTextProperty, value);
    }
    public QAView()
    {
        InitializeComponent();
    }

    private void QAButtonClicked(object sender, EventArgs e)
    {
        if (AnswerView.IsVisible)
        {
            AnswerView.IsVisible = false;
            DownDropdownButton.SetAppTheme<FileImageSource>(Image.SourceProperty, "right.png", "right_white.png");
        }
        else
        {
            AnswerView.IsVisible = true;
            DownDropdownButton.SetAppTheme<FileImageSource>(Image.SourceProperty, "down_dark.png", "down_light.png");
        }

    }
}