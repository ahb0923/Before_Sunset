public interface ICloseableUI
{
    /// <summary>
    /// UIManager.Instance.OpenUI();
    /// UI창을 열때 실질적으로 사용되는 메서드
    /// </summary>
    public void Open();

    /// <summary>
    /// UIManager.Instance.CloseUI();
    /// UI창을 닫을때 실질적으로 사용되는 메서드
    /// </summary>
    public void Close();
    
    /// <summary>
    /// RectTransform rect.OpenAtCenter();를 사용하여 구현
    /// UI창을 열때 사용하지 않음.
    /// </summary>
    public void OpenUI();
    
    /// <summary>
    /// RectTransform rect.CloseAndRestore();를 사용하여 구현
    /// UI창을 닫을때 사용하지 않음.
    /// </summary>
    public void CloseUI();
}
