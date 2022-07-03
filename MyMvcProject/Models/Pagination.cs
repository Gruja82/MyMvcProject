namespace MyMvcProject.Models
{
    public class Pagination<T>
    {
        public List<T> DataList { get; set; }
        public int CurrentPageIndex { get; set; }
        public int PageCount { get; set; }
    }
}
