namespace Test_backend.Models
{
    public class SearchByNamePaginationResponse
    {
        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int ElementsInPage { get; set; }

        public List<Meme> Data { get; set; }

        public SearchByNamePaginationResponse(int currentPage, int totalPages, int elementsInPage, List<Meme> data)
        {
            CurrentPage = currentPage;
            TotalPages = totalPages;
            ElementsInPage = elementsInPage;
            Data = data;
        }
    }
}
