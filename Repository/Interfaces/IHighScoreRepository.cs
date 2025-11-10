namespace Repository.Interfaces;

public interface IHighScoreRepository
{
    public List<HighScore> Get();
    public HighScore GetById();
    public bool Create();
    public bool Update();
    public bool Delete();
}