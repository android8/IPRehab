namespace IPRehab.Models
{
  public class StageCommandViewComponentTemplateModel
  {
    public RehabActionViewModel ActionVM { get; set; }
    public string Stage { get; set; }
    public string Title { get; set; }
    public string ActionBtnCssClass { get; set; }
    public string TextNode { get; set; }
    public bool ShowThisButton { get; set; }
    public bool DisableThisButton { get; set; }

    public StageCommandViewComponentTemplateModel()
    {
      ActionVM = new();
    }
  }
}