using WingjoyFramework.UIFramework.Runtime.Views;


public partial class Store_SelectKindMenuBtnsButtonAreaGroup : WingjoyFramework.UIFramework.Runtime.Views.BaseGroup
{
    
    private UnityEngine.UI.Button m_SoalFruit;
    
    private UnityEngine.UI.Button m_HeroCall;
    
    private UnityEngine.UI.Button m_MonterCome;
    
    private UnityEngine.UI.Button m_TreasureResource;
    
    public override void Init()
    {
        m_SoalFruit.onClick.AddListener(SoalFruit_BtnClick);
        m_HeroCall.onClick.AddListener(HeroCall_BtnClick);
        m_MonterCome.onClick.AddListener(MonterCome_BtnClick);
        m_TreasureResource.onClick.AddListener(TreasureResource_BtnClick);
    }
    
    public void SoalFruit_BtnClick()
    {
    }
    
    public void HeroCall_BtnClick()
    {
    }
    
    public void MonterCome_BtnClick()
    {
    }
    
    public void TreasureResource_BtnClick()
    {
    }
}
