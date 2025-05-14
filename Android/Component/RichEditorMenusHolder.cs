using Android.Attribute;
using Android.Helper;
using Android.Views;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;
using static Android.Views.ViewGroup;
using Color = Android.Graphics.Color;
using Drawable = Android.Graphics.Drawables.Drawable;

namespace Android.Component;

[ViewClassBind(Layout.rich_editor_menus)]
public class RichEditorMenusHolder(View view) : ViewHolder<View>(view)
{
    [ViewBind(Id.id_rich_editor_heading_layout)]
    public LinearLayout HeadingLayout;

    [ViewBind(Id.id_rich_editor_color)] public LinearLayout ColorLayout;

    [ViewBind(Id.id_rich_editor_heading_text)]
    public ImageView HeadingText;

    [ViewBind(Id.id_rich_editor_heading1)] public ImageView Heading1;

    [ViewBind(Id.id_rich_editor_heading2)] public ImageView Heading2;

    [ViewBind(Id.id_rich_editor_heading3)] public ImageView Heading3;

    [ViewBind(Id.id_rich_editor_heading4)] public ImageView Heading4;

    [ViewBind(Id.id_rich_editor_heading5)] public ImageView Heading5;

    [ViewBind(Id.id_rich_editor_heading6)] public ImageView Heading6;

    [ViewBind(Id.id_rich_editor_blockquote)]
    public ImageView Blockquote;

    protected override void Init()
    {
    }


    public void Bind(App.Activity activity, RichEditor editor, CommRichEditorTopMenusHolder? holder = null)
    {
        foreach (var ints in ColorHelper.Mp)
        {
            var drawable = ColorHelper.CreatDrawable(ints);
            var imageView = new ImageView(activity);
            imageView.SetImageDrawable(drawable);
            imageView.LayoutParameters = new LayoutParams(96, 96);
            imageView.SetPadding(4, 4, 4, 4);

            imageView.Click += ImageViewOnClick;
            ColorLayout.AddView(imageView);
        }

        HeadingLayout.Visibility = ViewStates.Gone;
        ColorLayout.Visibility = ViewStates.Gone;

        HeadingText.Click += HeadingOnClick;
        Heading1.Click += HeadingOnClick;
        Heading2.Click += HeadingOnClick;
        Heading3.Click += HeadingOnClick;
        Heading4.Click += HeadingOnClick;
        Heading5.Click += HeadingOnClick;
        Heading6.Click += HeadingOnClick;
        Blockquote.Click += HeadingOnClick;
        Blockquote.CallClick(editor.SetBlockquote);
        return;

        void HeadingOnClick(object? sender, EventArgs args)
        {
            if (sender is not ImageView i) return;
            var id = i.Id switch
            {
                Id.id_rich_editor_heading1 => 1,
                Id.id_rich_editor_heading2 => 2,
                Id.id_rich_editor_heading3 => 3,
                Id.id_rich_editor_heading4 => 4,
                Id.id_rich_editor_heading5 => 5,
                Id.id_rich_editor_heading6 => 6,
                _ => 1
            };
            holder?.Heading.SetImageDrawable(i.Drawable);
            editor.Heading = id;
            HeadingLayout.Visibility = ViewStates.Gone;
        }

        void ImageViewOnClick(object? sender, EventArgs args)
        {
            if (sender is not ImageView i) return;
            if (holder is not null)
            {
                var index = ColorLayout.IndexOfChild(i);
                if (holder.ColorFlag)
                {
                    editor.TextBackgroundColor = ColorHelper.Mp[index][0];
                }
                else
                {
                    editor.TextColor = ColorHelper.Mp[index][0];
                }
            }

            ColorLayout.Visibility = ViewStates.Gone;
        }
    }

    public Drawable? GetDrawable(RichEditor.DecorationStateEventArgs e) =>
        e.Types.Contains(RichEditor.RichType.H1) ? Heading1.Drawable :
        e.Types.Contains(RichEditor.RichType.H2) ? Heading2.Drawable :
        e.Types.Contains(RichEditor.RichType.H3) ? Heading3.Drawable :
        e.Types.Contains(RichEditor.RichType.H4) ? Heading4.Drawable :
        e.Types.Contains(RichEditor.RichType.H5) ? Heading5.Drawable :
        e.Types.Contains(RichEditor.RichType.H6) ? Heading6.Drawable :
        e.Types.Contains(RichEditor.RichType.Blockquote) ? Blockquote.Drawable :
        HeadingText.Drawable;
}

[ViewClassBind(Layout.rich_editor_bottom_menus)]
public class CommRichEditorBottomMenusHolder(View view) : ViewHolder<View>(view)
{
    [ViewBind(Id.id_rich_editor_undo)] public ImageView Undo;

    [ViewBind(Id.id_rich_editor_redo)] public ImageView Redo;

    [ViewBind(Id.id_rich_editor_insert_image)]
    public ImageView InsertImage;

    [ViewBind(Id.id_rich_editor_insert_link)]
    public ImageView InsertLink;

    [ViewBind(Id.id_rich_editor_insert_checkbox)]
    public ImageView InsertCheckbox;

    protected override void Init()
    {
    }

    public void Bind(RichEditor editor)
    {
        Undo.CallClick(editor.Undo);
        Redo.CallClick(editor.Redo);
        InsertCheckbox.CallClick(editor.InsertTodo);
    }
}

[ViewClassBind(Layout.rich_editor_top_menus)]
public class CommRichEditorTopMenusHolder(View view) : ViewHolder<View>(view)
{
    [ViewBind(Id.id_editor_menus_bold)] public ImageView Bold;

    [ViewBind(Id.id_editor_menus_italic)] public ImageView Italic;

    [ViewBind(Id.id_editor_menus_subscript)]
    public ImageView Subscript;

    [ViewBind(Id.id_editor_menus_superscript)]
    public ImageView Superscript;

    [ViewBind(Id.id_editor_menus_strikethrough)]
    public ImageView Strikethrough;

    [ViewBind(Id.id_editor_menus_underline)]
    public ImageView Underline;

    [ViewBind(Id.id_editor_menus_heading)] public ImageView Heading;

    [ViewBind(Id.id_editor_menus_txt_color)]
    public ImageView TextColor;

    [ViewBind(Id.id_editor_menus_bg_color)]
    public ImageView BgColor;

    [ViewBind(Id.id_editor_menus_indent)] public ImageView Indent;

    [ViewBind(Id.id_editor_menus_outdent)] public ImageView Outdent;

    [ViewBind(Id.id_editor_menus_align_left)]
    public ImageView AlignLeft;

    [ViewBind(Id.id_editor_menus_align_center)]
    public ImageView AlignCenter;

    [ViewBind(Id.id_editor_menus_align_right)]
    public ImageView AlignRight;

    [ViewBind(Id.id_editor_menus_insert_bullets)]
    public ImageView InsertBullets;

    [ViewBind(Id.id_editor_menus_insert_numbers)]
    public ImageView InsertNumbers;

    protected override void Init()
    {
    }

    public void Bind(RichEditor editor, RichEditorMenusHolder? holder = null)
    {
        Bold.CallClick(editor.SetBold);
        Italic.CallClick(editor.SetItalic);
        Superscript.CallClick(editor.SetSuperscript);
        Subscript.CallClick(editor.SetSubscript);
        Strikethrough.CallClick(editor.SetStrikeThrough);
        Underline.CallClick(editor.SetUnderline);
        Indent.CallClick(editor.SetIndent);
        Outdent.CallClick(editor.SetOutdent);
        AlignLeft.CallClick(editor.SetAlignLeft);
        AlignCenter.CallClick(editor.SetAlignCenter);
        AlignRight.CallClick(editor.SetAlignRight);
        InsertBullets.CallClick(editor.SetBullets);
        InsertNumbers.CallClick(editor.SetNumbers);

        editor.DecorationChanged += (sender, args) =>
        {
            var bg = Color.Black;
            var inBg = Color.Gray;

            Bold.SetColorFilter(args.Types.Contains(RichEditor.RichType.Bold) ? bg : inBg);

            Italic.SetColorFilter(args.Types.Contains(RichEditor.RichType.Italic) ? bg : inBg);
            Underline.SetColorFilter(args.Types.Contains(RichEditor.RichType.Underline) ? bg : inBg);
            Strikethrough.SetColorFilter(args.Types.Contains(RichEditor.RichType.Strikethrough) ? bg : inBg);

            Subscript.SetColorFilter(args.Types.Contains(RichEditor.RichType.Subscript) ? bg : inBg);
            Superscript.SetColorFilter(args.Types.Contains(RichEditor.RichType.Superscript) ? bg : inBg);


            var (ca, cb) =
                args.Types.Contains(RichEditor.RichType.Orderedlist) ? (bg, inBg)
                : args.Types.Contains(RichEditor.RichType.Unorderedlist) ? (inBg, bg)
                : (inBg, inBg);
            InsertNumbers.SetColorFilter(ca);
            InsertBullets.SetColorFilter(cb);

            var (c1, c2, c3) =
                args.Types.Contains(RichEditor.RichType.Justifycenter) ? (inBg, bg, inBg)
                : args.Types.Contains(RichEditor.RichType.Justifyright) ? (inBg, inBg, bg)
                : (bg, inBg, inBg);

            AlignLeft.SetColorFilter(c1);
            AlignCenter.SetColorFilter(c2);
            AlignRight.SetColorFilter(c3);

            if (holder is null) return;
            var textDrawable = holder?.GetDrawable(args);
            Heading.SetImageDrawable(textDrawable);
        };

        if (holder == null) return;
        TextColor.CallClick(() =>
        {
            holder.ColorLayout.Visibility = ViewStates.Visible;
            ColorFlag = false;
        });

        BgColor.CallClick(() =>
        {
            holder.ColorLayout.Visibility = ViewStates.Visible;
            ColorFlag = true;
        });

        Heading.CallClick(() => { holder.HeadingLayout.Visibility = ViewStates.Visible; });
    }

    public bool ColorFlag;
}