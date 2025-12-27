using System.Text;
using UnityEngine;

public class ItemStatus : MonoBehaviour
{
    ItemManager itemManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemManager = GetComponent<ItemManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

        public void SetStatus()
    {
        if (!itemManager.ItemList.activeSelf && itemManager.editItemStatus.activeSelf)
        {
            itemManager.statusItem.sprite = itemManager.editItem.Resource;
            itemManager.editItemName.text = $"아이템명 : {itemManager.editItem.Name}";

            itemManager.ItemStatus[0].text = $"등급 : {itemManager.editItem.Rank}";
            itemManager.ItemStatus[1].text = $"기본 공격력 증가 : {itemManager.editItem.AttackPower}";
            itemManager.ItemStatus[2].text = $"추가 공격력 : {itemManager.editItem.AdditionalAttackPower}%";
            itemManager.ItemStatus[3].text = $"방어력 감소 : {itemManager.editItem.NeutralizeDefense}";
            itemManager.ItemStatus[4].text = $"마법 증폭 : {itemManager.editItem.MagicalBuffer}%";
            itemManager.ItemStatus[5].text = $"마법방어력 감소 : {itemManager.editItem.MagicalDebuffer}%";
            itemManager.ItemStatus[6].text = $"방어무시 데미지 : {itemManager.editItem.TrueDamage * 100}%";
            itemManager.ItemStatus[7].text = $"체력 재생 : {DataManager.Instance.RoundX(itemManager.editItem.HealthRegen,3)}";
            itemManager.ItemStatus[8].text = $"마나 재생 : {DataManager.Instance.RoundX(itemManager.editItem.ManaRegen , 3)}";
            itemManager.ItemStatus[9].text = $"이동속도 감소 : {itemManager.editItem.MoveSpeed}";
            itemManager.ItemStatus[10].text = $"공격속도 증가 : {itemManager.editItem.AttackSpeed}%";
            itemManager.ItemStatus[11].text = $"타워 공격력 증가 : {itemManager.editItem.TowerDamage}";
            itemManager.ItemStatus[12].text = $"타워 공격속도 증가 : {itemManager.editItem.TowerAttackSpeed}%";
            itemManager.ItemStatus[13].text = $"공격 유형 : {itemManager.editItem.AttackType}";

            StringBuilder s = new StringBuilder();

            float Probability = itemManager.editItem.Probability;

            int MonoPhysics = itemManager.editItem.MonoPhysics;
            int MultiPhysics = itemManager.editItem.MultiPhysics;
            int MonoMagic = itemManager.editItem.MonoMagic;
            int MultiMagic = itemManager.editItem.MultiMagic;
            float MonoStun = itemManager.editItem.MonoStun;
            float MultiStun = itemManager.editItem.MultiStun;
            float Range = itemManager.editItem.Range;
            float Percent = itemManager.editItem.Percent;
            bool boss = itemManager.editItem.BossPercentAttack;
            float DoublePhysics = itemManager.editItem.DoublePhysics;
            float damageUp = itemManager.editItem.DamageUp;
            int PercentageCategory = (int)itemManager.editItem.PercentCategory;

            float attackRange = itemManager.editItem.AttackRange;

            PercentKind percentKind = itemManager.editItem.PercentKind;

            int Blink = itemManager.editItem.Blink;

            bool HaveASkill = false;

            if (Probability != 0)
            {
                s.AppendLine($"스킬 확률 : {Probability}%");
                if (MonoPhysics != 0) s.AppendLine($"단일 물리 데미지 : {MonoPhysics}");
                if (MultiPhysics != 0) s.AppendLine($"범위 물리 데미지 : {MultiPhysics}");
                if (MonoMagic != 0) s.AppendLine($"단일 마법 데미지 : {MonoMagic}");
                if (MultiMagic != 0) s.AppendLine($"범위 마법 데미지 : {MultiMagic}");
                if (MonoStun != 0) s.AppendLine($"단일 스턴 : {MonoStun}초");
                if (MultiStun != 0) s.AppendLine($"범위 스턴 : {MultiStun}초");
                if (Range != 0) s.AppendLine($"스킬 범위 : {Range * 100}");
                if (boss == true)
                {
                    s.AppendLine($"보스에게 {PercentCategory(PercentageCategory)}의 {Percent}%에 해당하는 데미지를 입힙니다");
                }
                else if (Percent != 0)
                {
                    if(percentKind == PercentKind.explosions)
                        s.AppendLine($"단일 대상에게 {PercentCategory(PercentageCategory)}의 {Percent}%에 해당하는 폭발형 데미지를 입힙니다. (보스몹, 스토리에겐 {Percent * DataManager.exPercent[PercentageCategory]}의 마법데미지를 입힙니다.)");
                    else
                        s.AppendLine($"{SkillRange(Range)} {PercentCategory(PercentageCategory)}의 {Percent}%에 해당하는 {SkillPercentKind(percentKind)} 데미지를 입힙니다. {(Range == 0 ? "(보스몹, 스토리 제외)" : "")}"); 
                }
                if (damageUp != 0) s.AppendLine($"치명타 : {damageUp * 100}%");

                switch ((itemManager.editItem.Name, itemManager.editItem.Rank))
                {
                    case ("좀비", ItemRank.안흔함):
                        s.AppendLine($"스킬 발동시 적 유닛이 사망하면 좀비 아이템 1개 추가");
                        break;
                }
                HaveASkill = true;

            }
            if (attackRange != 0)
            {
                s.AppendLine($"공격 범위(넓은 범위 우선) : {attackRange * 100}");
                if (DoublePhysics != 0) s.AppendLine($"공격력 비례 물리 데미지(짭플) : {DoublePhysics * 100}%");
                HaveASkill = true;
            }
            if(Blink != 0)
            {
                s.AppendLine($"도약의 사거리가 {Blink} 더 증가합니다.");
                HaveASkill = true;
            }
            if(!HaveASkill)
                s.AppendLine("스킬이 없습니다");


            itemManager.ItemSkillExplanation.text = s.ToString();
        }
    }


    private string PercentCategory(int percent)
    {
        switch(percent)
        {
            case 1:
            return "전체체력";
            case 2:
            return "현재체력";
            case 3:
            return "잃은체력";
            default:
            return "";
        }
    }

    private string SkillRange(float range)
    {
        switch(range)
        {
            case 0:
            return "단일 대상에게";
            default:
            return "범위에";
        }
    }    
    private string SkillPercentKind(PercentKind percentKind)
    {
        switch(percentKind)
        {
            case PercentKind.physics:
            return "물리";
            case PercentKind.magics:
            return "마법";
            case PercentKind.trueDamage:
            return "고정";
            default:
            return "";
        }
    }
}
