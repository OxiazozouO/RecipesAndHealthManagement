using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Helper;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Webkit;
using AnyLibrary.Helper;
using Java.Lang;
using Java.Net;
using static Android.Component.RichEditor.RichType;
using Exception = System.Exception;

namespace Android.Component;

public class RichEditor : WebView
{
    public enum RichType
    {
        Bold,
        Italic,
        Subscript,
        Superscript,
        Strikethrough,
        Underline,
        Blockquote,
        Div,
        H1,
        H2,
        H3,
        H4,
        H5,
        H6,
        Orderedlist,
        Unorderedlist,
        Justifycenter,
        Justifyfull,
        Justifyleft,
        Justifyright
    }

    public readonly List<RichType> DecorationTypes = new List<RichType>
    {
        Bold,
        Italic,
        Underline,
        Strikethrough,
        Blockquote,
        Div,
        H1,
        H2,
        H3,
        H4,
        H5,
        H6,
        Orderedlist,
        Unorderedlist,
        Justifyleft,
        Justifycenter,
        Justifyright,
        Justifyfull
    };

    public EventHandler<string>? TextChanged;
    public EventHandler<DecorationStateEventArgs>? DecorationChanged;
    public EventHandler<bool>? AfterInitialLoad;

    public class DecorationStateEventArgs(List<RichType> types) : EventArgs
    {
        public List<RichType> Types { get; } = types;
        public override string ToString() => string.Join(",", Types.Select(p => p.ToString()));

        public string TextColor { set; get; }
        public string TextBgColor { set; get; }
    }

    private bool _isReady = false;

    public bool IsReady
    {
        set
        {
            _isReady = value;
            if (value)
            {
                AfterInitialLoad?.Invoke(this, value);
            }
        }
    }


    private static readonly string SetupHtml = "file:///android_asset/editor.html";
    private static readonly string CallbackScheme = "re-callback://";
    private static readonly string StateScheme = "re-state://";
    private string _mContents;

    private RichEditor(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {
    }

    public RichEditor(Context context, IAttributeSet? attrs, int defStyleAttr, bool privateBrowsing) : base(context,
        attrs, defStyleAttr, privateBrowsing)
    {
    }

    public RichEditor(Context context, IAttributeSet? attrs, int defStyleAttr, int defStyleRes) : base(context, attrs,
        defStyleAttr, defStyleRes)
    {
    }

    public RichEditor(Context context, IAttributeSet? attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
    {
        Init();
        ApplyAttributes(context, attrs);
    }

    public void Init()
    {
        VerticalScrollBarEnabled = false;
        HorizontalScrollBarEnabled = false;
        Settings.JavaScriptEnabled = true;
        Focusable = true;
        FocusableInTouchMode = true;

        SetWebChromeClient(new WebChromeClient());
        SetWebViewClient(new EditorWebViewClient());
        LoadUrl(SetupHtml);
    }

    public RichEditor(Context context, IAttributeSet? attrs) : this(context, attrs, Resource.Attribute.badgeStyle)
    {
    }

    public RichEditor(Context context) : this(context, null)
    {
    }

    private void Callback(string text)
    {
        _mContents = text.Replace(CallbackScheme, "", StringComparison.CurrentCultureIgnoreCase);
        TextChanged?.Invoke(this, _mContents);
    }

    private string StateCheck
    {
        set
        {
            var state = value
                .Replace(StateScheme, "", StringComparison.CurrentCultureIgnoreCase)
                .ToLower()
                .Split(',')
                .ToList();

            var types = DecorationTypes
                .Where(type => state.Contains(type.ToString().ToLower())).ToList();

            var args = new DecorationStateEventArgs(types);
            int ind = state.IndexOf("textColor");
            if (ind > 0) args.TextColor = state[ind + 1];
            ind = state.IndexOf("bgColor");
            if (ind > 0) args.TextBgColor = state[ind + 1];

            DecorationChanged?.Invoke(this, args);
        }
    }

    private void ApplyAttributes(Context context, IAttributeSet? attrs)
    {
        int[] attrsArray = [Resource.Attribute.buttonGravity];
        TypedArray ta = context.ObtainStyledAttributes(attrs, attrsArray);

        var gravity = (GravityFlags)ta.GetInt(0, NoId);
        switch (gravity)
        {
            case GravityFlags.Left:
                Exec("setTextAlign(\"left\")");
                break;
            case GravityFlags.Right:
                Exec("setTextAlign(\"right\")");
                break;
            case GravityFlags.Top:
                Exec("setVerticalAlign(\"top\")");
                break;
            case GravityFlags.Bottom:
                Exec("setVerticalAlign(\"bottom\")");
                break;
            case GravityFlags.CenterVertical:
                Exec("setVerticalAlign(\"middle\")");
                break;
            case GravityFlags.CenterHorizontal:
                Exec("setTextAlign(\"center\")");
                break;
            case GravityFlags.Center:
                Exec("setVerticalAlign(\"middle\")");
                Exec("setTextAlign(\"center\")");
                break;
        }

        ta.Recycle();
    }

    public string Html
    {
        get => _mContents;
        set
        {
            value ??= "";

            try
            {
                Exec($"setHtml('{URLEncoder.Encode(value, "UTF-8")}')");
            }
            catch (Exception)
            {
                // ignored
            }

            _mContents = value;
        }
    }


    public int EditorFontColor
    {
        set => Exec($"setTextColor('{value.ToHexColor()}')");
    }


    public int EditorFontSize
    {
        set => Exec($"setBaseFontSize('{value}px')");
    }

    public void SetReadOnly() => Exec($"setReadOnly()");

    public override void SetPadding(int left, int top, int right, int bottom)
    {
        base.SetPadding(left, top, right, bottom);
        Exec($"setPadding('{left}px', '{top}px', '{right}px', '{bottom}px')");
    }

    public override void SetPaddingRelative(int start, int top, int end, int bottom)
    {
        SetPadding(start, top, end, bottom);
    }

    public Color EditorBackgroundColor
    {
        set => SetBackgroundColor(value);
    }

    public override void SetBackgroundResource(int resid)
    {
        Bitmap bitmap = UnescapeDataStringResource(Context, resid);
        string base64 = ViewHelper.ToBase64(bitmap);
        bitmap.Recycle();

        Exec($"setBackgroundImage('url(data:image/png;base64,{base64})')");
    }

    public override Drawable? Background
    {
        get => base.Background;
        set
        {
            Bitmap bitmap = ToBitmap(value);
            string base64 = ViewHelper.ToBase64(bitmap);
            bitmap.Recycle();

            Exec($"setBackgroundImage('url(data:image/png;base64,{base64})')");
        }
    }

    public void SetBackground(string url)
    {
        Exec($"setBackgroundImage('url({url})')");
    }

    public int EditorWidth
    {
        set => Exec($"setWidth('{value}px')");
    }

    public int EditorHeight
    {
        set => Exec($"setHeight('{value}px')");
    }


    public string Placeholder
    {
        set => Exec($"setPlaceholder('{value}')");
    }

    public bool InputEnabled
    {
        set => Exec($"setInputEnabled({value})");
    }

    public void Undo() => Exec("undo()");

    public void Redo() => Exec("redo()");

    public void SetBold() => Exec("setBold()");

    public void SetItalic() => Exec("setItalic()");

    public void SetSubscript() => Exec("setSubscript()");

    public void SetSuperscript() => Exec("setSuperscript()");

    public void SetStrikeThrough() => Exec("setStrikeThrough()");

    public void SetUnderline() => Exec("setUnderline()");

    public int TextColor
    {
        set
        {
            Exec("prepareInsert()");
            Exec($"setTextColor('{value.ToHexColor()}')");
        }
    }

    public int TextBackgroundColor
    {
        set
        {
            Exec("prepareInsert()");
            Exec($"setTextBackgroundColor('{value.ToHexColor()}')");
        }
    }

    public int FontSize
    {
        set
        {
            if (value is > 7 or < 1)
            {
                Log.Error("RichEditor", "Font size should have a value between 1-7");
            }

            Exec($"setFontSize('{value}')");
        }
    }

    public int Heading
    {
        set => Exec($"setHeading('{value}')");
    }

    public void RemoveFormat() => Exec("removeFormat()");


    public void SetIndent() => Exec("setIndent()");

    public void SetOutdent() => Exec("setOutdent()");

    public void SetAlignLeft() => Exec("setJustifyLeft()");

    public void SetAlignCenter() => Exec("setJustifyCenter()");

    public void SetAlignRight() => Exec("setJustifyRight()");

    public void SetBlockquote() => Exec("setBlockquote()");

    public void SetBullets() => Exec("setBullets()");

    public void SetNumbers()
    {
        Exec("setNumbers()");
    }

    public void InsertImage(string tag, string url, string alt, int width, int height = 0)
    {
        Exec("prepareInsert()");
        Exec(height == 0
            ? $"insertImageW('{tag}', '{url}', '{alt}','{width}')"
            : $"insertImageWH('{tag}', '{url}', '{alt}','{width}', '{height}')");
    }

    public void InsertVideo(string url, int width = 0, int height = 0)
    {
        Exec("prepareInsert()");
        if (width == 0)
        {
            Exec($"insertVideo('{url}')");
        }
        else if (height == 0)
        {
            Exec($"insertVideoW('{url}', '{width}')");
        }
        else
        {
            Exec($"insertVideoWH('{url}', '{width}', '{height}')");
        }
    }

    public void InsertAudio(string url)
    {
        Exec("prepareInsert()");
        Exec($"insertAudio('{url}')");
    }

    public void InsertLink(string href, string title)
    {
        Exec("prepareInsert()");
        Exec($"insertLink('{href}', '{title}')");
    }

    public void InsertTab(string src, string title, string refer, int idCategory, int id)
    {
        Exec("prepareInsert()");
        Exec($"insertTab('{src}', '{title}', '{refer}', '{idCategory}_{id}')");
    }

    public void InsertTodo()
    {
        Exec("prepareInsert()");
        Exec($"setTodo('{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}')");
    }

    public void FocusEditor()
    {
        RequestFocus();
        Exec("focus()");
    }

    public void ClearFocusEditor() => Exec("blurFocus()");

    private void Exec(string trigger)
    {
        trigger = $"javascript:RE.{trigger};";
        if (_isReady)
        {
            Load(trigger);
        }
        else
        {
            PostDelayed(new Runnable(() => { Exec(trigger); }), 100);
        }
    }

    [Obsolete("Obsolete")]
    private void Load(string trigger)
    {
        if (Build.VERSION.SdkInt >= Build.VERSION_CODES.Kitkat)
        {
            EvaluateJavascript(trigger, null);
        }
        else
        {
            LoadUrl(trigger);
        }
    }

    private class EditorWebViewClient : WebViewClient
    {
        public override void OnPageFinished(WebView? view, string? url)
        {
            if (view is not RichEditor r) return;
            bool b = string.Equals(url, SetupHtml, StringComparison.OrdinalIgnoreCase);
            r.IsReady = b;
        }

        public override bool ShouldOverrideUrlLoading(WebView? view, string? url)
        {
            if (view is not RichEditor r) return false;
            string unescapeDataString = Uri.UnescapeDataString(url);

            if (TextUtils.IndexOf(url, CallbackScheme) == 0)
            {
                r.Callback(unescapeDataString);
                return true;
            }

            if (TextUtils.IndexOf(url, StateScheme) != 0)
                return base.ShouldOverrideUrlLoading(view, url);

            r.StateCheck = unescapeDataString;
            return true;
        }

        public override bool ShouldOverrideUrlLoading(WebView? view, IWebResourceRequest? request)
        {
            if (view is not RichEditor r) return false;
            string url = request.Url.ToString();
            string unescapeDataString = Uri.UnescapeDataString(url);

            if (TextUtils.IndexOf(url, CallbackScheme) == 0)
            {
                r.Callback(unescapeDataString);
                return true;
            }

            if (TextUtils.IndexOf(url, StateScheme) != 0)
                return base.ShouldOverrideUrlLoading(view, request);
            r.StateCheck = unescapeDataString;
            return true;
        }
    }

    public static Bitmap ToBitmap(Drawable drawable)
    {
        if (drawable is BitmapDrawable d)
        {
            return d.Bitmap;
        }

        int width = drawable.IntrinsicWidth;
        width = width > 0 ? width : 1;
        int height = drawable.IntrinsicHeight;
        height = height > 0 ? height : 1;

        Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
        Canvas canvas = new Canvas(bitmap);
        drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
        drawable.Draw(canvas);

        return bitmap;
    }

    public static Bitmap UnescapeDataStringResource(Context context, int resId)
    {
        return BitmapFactory.DecodeResource(context.Resources, resId);
    }
}