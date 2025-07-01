public class MonsterDataHandler : BaseDataHandler<MonsterData>
{
    protected override string FileName => "MonsterData_JSON.json";
    protected override int GetId(MonsterData data) => data.id;
    protected override string GetName(MonsterData data) => data.monsterName;
}
