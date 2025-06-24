using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static UnityEditor.Progress;


public interface IDataLoader
{
    Task LoadAsync();
}

public class DataManager : PlainSingleton<DataManager>
{
    public ItemDataManager ItemData { get; private set; }
    public TowerDataManager TowerData { get; private set; }
    public MonsterDataManager MonsterData { get; private set; }




    public void Init()
    {
        List<IDataLoader> loaders = new() { ItemData, TowerData, MonsterData };

       /* foreach (var loader in loaders)
            await loader.LoadAsync();*/

    }
}
