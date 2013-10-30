using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BlowTrial.WpfProperties
{
public sealed class MaskedText : IDisposable
{
    private readonly TextBox _target;
    private MaskedTextProvider _provider;

    private MaskedText(TextBox target)
    {
        _target = target;
        _target.PreviewTextInput += OnPreviewTextInput;
        CreateProvider();
    }

    public void Dispose()
    {
        _target.PreviewTextInput -= OnPreviewTextInput;
        GC.SuppressFinalize(this);
    }

    private void OnPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
        e.Handled = true;
        ReplaceText(e.Text);
    }

    private void CreateProvider()
    {
        string text = _target.Text;
        if (_provider != null)
        {
            text = _provider.ToString(false, false);
        }

        _provider = new MaskedTextProvider(
            GetMask(_target),
            CultureInfo.CurrentCulture,
            true, // allow prompt as input
            GetPromptChar(_target),
            '*',
            false); // Ascii only

        int testPosition;
        MaskedTextResultHint hint;
        _provider.Replace(text, 0, _provider.Length, out testPosition, out hint);
        SetText();
    }

    private void SetText()
    {
        int start = _target.SelectionStart;
        _target.Text = GetFormattedDisplayString();
        _target.SelectionStart = start;
        _target.SelectionLength = 0;
    }

    private string GetFormattedDisplayString()
    {
        bool flag;
        if (_target.IsReadOnly)
        {
            flag = false;
        }
        else if (DesignerProperties.GetIsInDesignMode(_target))
        {
            flag = true;
        }
        else
        {
            flag = !GetHidePromptOnLeave(_target) || _target.IsFocused;
        }

        return _provider.ToString(false, flag, true, 0, _provider.Length);
    }

    private void ReplaceText(string text)
    {
        int start = _target.SelectionStart;
        int end = start + _target.SelectionLength - 1;

        int testPosition;
        MaskedTextResultHint hint;
        if (end >= start)
        {
            _provider.Replace(text, start, end, out testPosition, out hint);
        }
        else
        {
            _provider.InsertAt(text, start, out testPosition, out hint);
        }

        if (hint == MaskedTextResultHint.Success ||
            hint == MaskedTextResultHint.CharacterEscaped ||
            hint == MaskedTextResultHint.NoEffect ||
            hint == MaskedTextResultHint.SideEffect)
        {
            SetText();
            _target.SelectionStart = testPosition + text.Length;
            _target.SelectionLength = 0;
        }
        else
        {
            //RaiseMaskInputRejectedEvent(hint, start);
        }
    }

    #region Instance

    private static readonly DependencyProperty InstanceProperty =
        DependencyProperty.RegisterAttached("Instance", typeof(MaskedText), typeof(MaskedText),
            new FrameworkPropertyMetadata((MaskedText)null));

    private static MaskedText GetInstance(DependencyObject d)
    {
        return (MaskedText)d.GetValue(InstanceProperty);
    }

    private static void SetInstance(DependencyObject d, MaskedText value)
    {
        d.SetValue(InstanceProperty, value);
    }

    #endregion

    #region Mask

    public static readonly DependencyProperty MaskProperty =
        DependencyProperty.RegisterAttached(
            "Mask",
            typeof(string),
            typeof(MaskedText),
            new FrameworkPropertyMetadata((string)null, new PropertyChangedCallback(OnMaskChanged), new CoerceValueCallback(CoerceMaskValue)),
            new ValidateValueCallback(IsMaskValid));

    public static string GetMask(DependencyObject d)
    {
        return (string)d.GetValue(MaskProperty);
    }

    public static void SetMask(DependencyObject d, string value)
    {
        d.SetValue(MaskProperty, value);
    }

    private static void OnMaskChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue == null)
        {
            Unattach(d);
        }
        else
        {
            Attach(d);
        }
    }

    private static object CoerceMaskValue(DependencyObject d, object value)
    {
        string mask = (string)value;
        if (string.IsNullOrEmpty(mask))
        {
            return null;
        }
        return value;
    }

    private static bool IsMaskValid(object value)
    {
        string mask = (string)value;
        if (string.IsNullOrEmpty(mask))
        {
            return true;
        }

        foreach (char ch in mask)
        {
            if (!MaskedTextProvider.IsValidMaskChar(ch))
            {
                return false;
            }
        }

        return true;
    }

    #endregion

    #region PromptChar

    public static readonly DependencyProperty PromptCharProperty =
        DependencyProperty.RegisterAttached(
            "PromptChar",
            typeof(char),
            typeof(MaskedText),
            new FrameworkPropertyMetadata('_', FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnPromptCharChanged)),
            new ValidateValueCallback(IsPromptCharValid));

    public static char GetPromptChar(DependencyObject d)
    {
        return (char)d.GetValue(PromptCharProperty);
    }

    public static void SetPromptChar(DependencyObject d, char value)
    {
        d.SetValue(PromptCharProperty, value);
    }

    private static void OnPromptCharChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        MaskedText maskedText = GetInstance(d);
        if (maskedText != null)
        {
            maskedText._provider.PromptChar = (char)e.NewValue;
            maskedText.SetText();
        }
    }

    private static bool IsPromptCharValid(object value)
    {
        char ch = (char)value;
        return MaskedTextProvider.IsValidPasswordChar(ch);
    }

    #endregion

    #region IncludePrompt

    public static readonly DependencyProperty IncludePromptProperty =
        DependencyProperty.RegisterAttached(
            "IncludePrompt",
            typeof(bool),
            typeof(MaskedText),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIncludePromptChanged)));

    public static bool GetIncludePrompt(DependencyObject d)
    {
        return (bool)d.GetValue(IncludePromptProperty);
    }

    public static void SetIncludePrompt(DependencyObject d, bool value)
    {
        d.SetValue(IncludePromptProperty, value);
    }

    private static void OnIncludePromptChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        MaskedText maskedText = GetInstance(d);
        if (maskedText != null)
        {
            maskedText.CreateProvider();
        }
    }

    #endregion

    #region HidePromptOnLeave

    public static readonly DependencyProperty HidePromptOnLeaveProperty =
        DependencyProperty.RegisterAttached(
            "HidePromptOnLeave",
            typeof(bool),
            typeof(MaskedText),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnHidePromptOnLeaveChanged)));

    public static bool GetHidePromptOnLeave(DependencyObject d)
    {
        return (bool)d.GetValue(HidePromptOnLeaveProperty);
    }

    public static void SetHidePromptOnLeave(DependencyObject d, bool value)
    {
        d.SetValue(HidePromptOnLeaveProperty, value);
    }

    private static void OnHidePromptOnLeaveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        MaskedText maskedText = GetInstance(d);
        if (maskedText != null)
        {
            maskedText.SetText();
        }
    }

    #endregion

    private static void Attach(DependencyObject d)
    {
        TextBox textBox = d as TextBox;
        if (textBox != null)
        {
            MaskedText maskedText = GetInstance(d);
            if (maskedText == null)
            {
                maskedText = new MaskedText(textBox);
                SetInstance(d, maskedText);
            }
            else
            {
                maskedText.CreateProvider();
            }
        }
    }

    private static void Unattach(DependencyObject d)
    {
        MaskedText maskedText = GetInstance(d);
        if (maskedText != null)
        {
            maskedText.Dispose();
            SetInstance(d, null);
        }
    }
}
}
