using Android.Attribute;
using Android.Helper;
using Android.HttpClients;
using Android.Views;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.msg_edit_nutrient)]
public class MsgEditNutrientHelper(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_nutrient_edit_fat_normal)]
    public SeekBar FatNormal;

    [ViewBind(Id.id_nutrient_edit_fat_percentage)]
    public TextView FatPercentage;

    [ViewBind(Id.id_nutrient_edit_fat_requirement)]
    public TextView FatRequirement;

    [ViewBind(Id.id_nutrient_edit_carbohydrate_normal)]
    public SeekBar CarbohydrateNormal;

    [ViewBind(Id.id_nutrient_edit_carbohydrate_percentage)]
    public TextView CarbohydratePercentage;

    [ViewBind(Id.id_nutrient_edit_carbohydrate_requirement)]
    public TextView CarbohydrateRequirement;

    [ViewBind(Id.id_nutrient_edit_protein_normal)]
    public SeekBar ProteinNormal;

    [ViewBind(Id.id_nutrient_edit_protein_percentage)]
    public TextView ProteinPercentage;

    [ViewBind(Id.id_nutrient_edit_protein_requirement)]
    public TextView ProteinRequirement;

    [ViewBind(Id.id_nutrient_edit_protein_rni)]
    public TextView ProteinRni;

    [ViewBind(Id.id_nutrient_edit_tdee_correct)]
    public TextView TdeeCorrect;

    [ViewBind(Id.id_nutrient_edit_tdee)] public TextView Tdee;

    [ViewBind(Id.id_nutrient_edit_tdee_correct_normal)]
    public SeekBar TdeeCorrectNormal;

    protected override void Init()
    {
        int flag = 0;
        var model = AppConfigHelper.MyInfo;
        model.PhysicalList = ApiService.GetMyAllInfo();

        TdeeCorrectNormal.ProgressChanged += (sender, args) =>
        {
            if (flag is 1) return;
            flag = 1;
            var value = args.Progress;
            AppConfigHelper.TdeeCorrect = value / 100f;

            TdeeCorrect.Text = $"每天摄取TDEE×{value:#.##}%  =  {(model.Physical.TdeeGroup.Tdee * AppConfigHelper.TdeeCorrect):#.##} kcal";

            Update();
            flag = 0;
        };


        FatNormal.ProgressChanged += (sender, args) =>
        {
            if (flag is 1) return;
            flag = 1;
            var value = args.Progress;
            model.Physical.FatPercentage = value / 10000f;
            Update();
            flag = 0;
        };
        ProteinNormal.ProgressChanged += (sender, args) =>
        {
            if (flag is 1) return;
            flag = 1;
            var value = args.Progress;
            model.Physical.ProteinPercentage = value / 10000f;
            Update();
            flag = 0;
        };
        ProteinRni.CallClick(() =>
        {
            model.Physical.ProteinPercentage = 0;
            Update();
        });

        Update();
        return;

        void Update()
        {
            model.Physical.Bind(model);
            Tdee.Text = $"每日能量总耗(TDEE) {(model.Physical.TdeeGroup.Tdee):#.##} kcal";
            ProteinRni.Text = $"推荐蛋白质摄入量 {model.Physical.ProteinRni:#.##} g/天 点击填入";

            TdeeCorrectNormal.Progress = (int)(AppConfigHelper.TdeeCorrect * 100);
            FatNormal.Progress = (int)(model.Physical.FatPercentage * 10000);
            ProteinNormal.Progress = (int)(model.Physical.ProteinPercentage * 10000);
            CarbohydrateNormal.Progress = (int)(model.Physical.CarbohydratePercentage * 10000);

            FatPercentage.Text = $"{model.Physical.FatPercentage * 100:#.##}%";
            CarbohydratePercentage.Text = $"{model.Physical.CarbohydratePercentage * 100:#.##}%";
            ProteinPercentage.Text = $"{model.Physical.ProteinPercentage * 100:#.##}%";

            FatRequirement.Text = $"{model.Physical.FatRequirement:#.##} g/天";
            CarbohydrateRequirement.Text = $"{model.Physical.CarbohydrateRequirement:#.##} g/天";
            ProteinRequirement.Text = $"{model.Physical.ProteinRequirement:#.##} g/天";
        }
    }
}