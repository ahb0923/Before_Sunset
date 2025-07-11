public class MonsterDataHandler : BaseDataHandler<MonsterDatabase>
{
    protected override string FileName => "MonsterData_JSON.json";
    protected override int GetId(MonsterDatabase data) => data.id;
    protected override string GetName(MonsterDatabase data) => data.monsterName;

}
