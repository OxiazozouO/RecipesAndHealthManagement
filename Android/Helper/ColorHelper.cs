using Android.Graphics.Drawables;

namespace Android.Helper;

// @formatter:off
public static class ColorHelper
{
    
    static ColorHelper()
    {
        Blue      = [0x47CBCF, 0x63BCCC];
        Purple    = [0xCEB4D7, 0xE7B7C7];
        Orange    = [0xFFBD89, 0xECBAB9];
        OrangeRed = [0xfda085, 0xff9a9e];
        A         = [0x7aff0c, 0xc2ff0c];
        B         = [0xc2ff0c, 0xfff70c];
        C         = [0xffda0c, 0xffbe0c];
        D         = [0x516BD7, 0x6DAFD6];
        E         = [0x84fab0, 0x8fd3f4];
        F         = [0x5ee7df, 0xb490ca];
        G         = [0x9890e3, 0xb1f4cf];
        H         = [0xa8e063, 0x56ab2f];
        I         = [0x2af598, 0x009efd];
        J         = [0xa1c4fd, 0xc2e9fb];
        K         = [0x48c6ef, 0x6f86d6];
        L         = [0xfeada6, 0xf5efef];
        N         = [0xaccbee, 0xe7f0fd];
        O         = [0xe9defa, 0xfbfcdb];
        P         = [0xc1dfc4, 0xdeecdd];
        Q         = [0x0ba360, 0x3cba92];
        S         = [0x74ebd5, 0x9face6];
        T         = [0x6a85b6, 0xbac8e0];
        U         = [0xa3bded, 0x6991c7];
        V         = [0x9795f0, 0xfbc8d4];
        W         = [0xa7a6cb, 0x8989ba];
        X         = [0xd9afd9, 0x97d9e1];
        Y         = [0x93a5cf, 0xe4efe9];
        Z         = [0x92fe9d, 0x00c9ff];
        M         = [0x616161, 0x5d4157];
        R         = [0x8baaaa, 0xae8b9c];
        A1        = [0xFFFEFF, 0xD7FFFE];
        Hid       = [0x00FFFFFF, 0x00FFFFFF];

        Mp = [
            [0x000000, 0x000000],
            [0xFFFEFE, 0xFFFEFE],
            [0xFF0000, 0xFF0000],
            [0x00FF00, 0x00FF00],
            [0x0000FF, 0x0000FF],
            [0xFFFF00, 0xFFFF00],
            Blue, Purple, Orange, OrangeRed,
            A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, A1,
            [0xabecd6, 0xfbed96],
            [0xddd6f3, 0xfaaca8],
            [0xdcb0ed, 0x99c99c],
            [0xf3e7e9, 0xe3eeff],
            [0x96deda, 0x50c9c3],
            [0xa8caba, 0x5d4157],
            [0x00cdac, 0x8ddad5],
            [0xf794a4, 0xfdd6bd],
            [0x64b3f4, 0xc2e59c],
            [0x0fd850, 0xf9f047],
            [0xee9ca7, 0xffdde1],
            [0x209cff, 0x68e0cf],
            [0xbdc2e8, 0xe6dee9],
            [0xe6b980, 0xeacda3],
            [0x9be15d, 0x00e3ae],
            [0xed6ea0, 0xec8c69],
            [0xffc3a0, 0xffafbd],
            [0xdfe9f3, 0xffffff],
            [0x50cc7f, 0xf5d100],
            [0x0acffe, 0x495aff],
            [0x616161, 0x9bc5c3],
            [0xdf89b5, 0xbfd9fe],
            [0xed6ea0, 0xec8c69],
            [0xc1c161, 0xd4d4b1],
            [0xec77ab, 0x7873f5],
            [0x007adf, 0x00ecbc],
            [0x20E2D7, 0xF9FEA5],
            [0xB6CEE8, 0xF578DC],
            [0xE3FDF5, 0xFFE6FA],
            [0x7DE2FC, 0xB9B6E5],
            [0xCBBACC, 0x2580B3],
            [0xB7F8DB, 0x50A7C2]
        ];
    }

    public static readonly List<int[]> Mp;

    public static readonly int[] Blue,Purple,Orange,OrangeRed,
        A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, A1,Hid;
// @formatter:on
    public static int[] RandomColor
    {
        get
        {
            var ind = new Random().Next(0, Mp.Count);
            return Mp[ind];
        }
    }

    public static int[] GetColor(decimal value, decimal ear, decimal rni, decimal ul)
    {
        decimal a, b, c;
        a = b = c = 0;
        var flag = (ear > 0 ? 1 : 0) + (rni > 0 ? 2 : 0) + (ul > 0 ? 4 : 0);

        switch (flag)
        {
            case 0:
                return P;
            case 1:
                a = ear;
                b = ear * 1.2m;
                c = ear * 1.6m;
                break;
            case 2:
                a = rni * 0.8m;
                b = rni;
                c = rni * 1.4m;
                break;
            case 3:
                a = ear;
                b = rni;
                c = rni * 1.4m;
                break;
            case 4:
                a = ul * 0.4m;
                b = ul * 0.6m;
                c = ul;
                break;
            case 5:
                a = ear;
                b = Math.Min(ear * 1.2m, ul * 0.6m);
                c = ul;
                break;
            case 6:
                a = rni * 0.8m;
                b = rni;
                c = ul;
                break;
            case 7:
                a = ear;
                b = rni;
                c = ul;
                break;
        }

        if (value < a) return Y;

        if (value < b) return C;

        if (value < c)
        {
            var l = Math.Min(b * 1.2m, c * 0.6m);
            var r = Math.Max(b * 1.6m, c * 0.8m);
            if (value < l)
                return A;
            if (value < r)
                return Q;
            return OrangeRed;
        }

        return OrangeRed;
    }

    public static GradientDrawable CreatDrawable(int[] colors)
    {
        var drawable = new GradientDrawable();
        drawable.SetShape(ShapeType.Rectangle);
        drawable.SetCornerRadius(100);
        drawable.SetGradientType(GradientType.LinearGradient);
        drawable.SetColors(colors.GetAndroidColor());
        drawable.SetOrientation(GradientDrawable.Orientation.LeftRight);
        return drawable;
    }

    public static int[] GetAndroidColor(this int[] pa)
    {
        int[] res = new int[pa.Length];
        for (var i = pa.Length - 1; i >= 0; i--)
            res[i] = pa[i] - 0xffffff;
        return res;
    }
}