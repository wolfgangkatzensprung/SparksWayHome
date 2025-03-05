public class EscapeMenu : BaseMenuPanel
{
    protected override void OnDisable()
    {
        base.OnDisable();
        Game.Unpause();
        Game.HideCursor();
    }
}
