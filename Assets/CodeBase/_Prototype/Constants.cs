namespace CodeBase._Prototype
{
  public static class Constants
  {
    #region Scene Names

    public const string BootstrapSceneName = "Bootstap"; // инит сцена, по сути 0 сцена
    public const string MenuSceneName = "Menu"; // нужна будет на демке
    public const string HubSceneName = "Hub"; // лобак, попдаем сюда после успешного подключения к серваку
    public const string TestSceneName = "Test"; // тут основные механики для тевстов

    #endregion

    #region Scene index

    public const int BootstrapSceneIndex = 0;
    public const int MenuSceneIndex = 1;
    public const int HubSceneIndex = 2;
    public const int TestSceneIndex = 1;

    #endregion
  }
}