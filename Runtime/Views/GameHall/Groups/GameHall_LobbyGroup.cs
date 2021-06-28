using WingjoyFramework.UIFramework.Runtime.Views;


public partial class GameHall_LobbyButtonAreaGroup : WingjoyFramework.UIFramework.Runtime.Views.BaseGroup
{
    
    private UnityEngine.UI.Button m_HeadInLobby;
    
    private UnityEngine.UI.Button m_Setup_Button;
    
    private UnityEngine.UI.Button m_Shop_Button;
    
    private UnityEngine.UI.Button m_Activity_Button;
    
    private UnityEngine.UI.Button m_Mission_Button;
    
    private UnityEngine.UI.Button m_Handbook_Button;
    
    private UnityEngine.UI.Button m_Train_Button;
    
    private UnityEngine.UI.Button m_Friend_Button;
    
    private UnityEngine.UI.Button m_Bag_Button;
    
    private UnityEngine.UI.Button m_FightPrepare_Button;
    
    private UnityEngine.UI.Button m_MatchStart;
    
    private UnityEngine.UI.Button m_MatchCancel;
    
    private UnityEngine.UI.Button m_Quiteam;
    
    private UnityEngine.UI.Button m_Help_Button;
    
    public override void Init()
    {
        m_HeadInLobby.onClick.AddListener(HeadInLobby_BtnClick);
        m_Setup_Button.onClick.AddListener(Setup_Button_BtnClick);
        m_Shop_Button.onClick.AddListener(Shop_Button_BtnClick);
        m_Activity_Button.onClick.AddListener(Activity_Button_BtnClick);
        m_Mission_Button.onClick.AddListener(Mission_Button_BtnClick);
        m_Handbook_Button.onClick.AddListener(Handbook_Button_BtnClick);
        m_Train_Button.onClick.AddListener(Train_Button_BtnClick);
        m_Friend_Button.onClick.AddListener(Friend_Button_BtnClick);
        m_Bag_Button.onClick.AddListener(Bag_Button_BtnClick);
        m_FightPrepare_Button.onClick.AddListener(FightPrepare_Button_BtnClick);
        m_MatchStart.onClick.AddListener(MatchStart_BtnClick);
        m_MatchCancel.onClick.AddListener(MatchCancel_BtnClick);
        m_Quiteam.onClick.AddListener(Quiteam_BtnClick);
        m_Help_Button.onClick.AddListener(Help_Button_BtnClick);
    }
    
    public void HeadInLobby_BtnClick()
    {
    }
    
    public void Setup_Button_BtnClick()
    {
    }
    
    public void Shop_Button_BtnClick()
    {
    }
    
    public void Activity_Button_BtnClick()
    {
    }
    
    public void Mission_Button_BtnClick()
    {
    }
    
    public void Handbook_Button_BtnClick()
    {
    }
    
    public void Train_Button_BtnClick()
    {
    }
    
    public void Friend_Button_BtnClick()
    {
    }
    
    public void Bag_Button_BtnClick()
    {
    }
    
    public void FightPrepare_Button_BtnClick()
    {
    }
    
    public void MatchStart_BtnClick()
    {
    }
    
    public void MatchCancel_BtnClick()
    {
    }
    
    public void Quiteam_BtnClick()
    {
    }
    
    public void Help_Button_BtnClick()
    {
    }
}
