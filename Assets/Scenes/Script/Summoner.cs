using System.Collections;
using TMPro;
using UnityEngine;

public class Summoner : MonoBehaviour
{
    public GameObject characterPrefab;        // 소환할 캐릭터 프리팹
    public GameObject NormalUnit;
    private float summonInterval;         // 소환 주기
    private Coroutine summonRoutine;

    public PlayerStats player;
    public TextMeshProUGUI tmp;

    void Awake()
    {

    }
    void Start()
    {
        if (summonInterval == 0)
            summonInterval = 0.6f;

    }

    public EnemyStats BossSummoner(GameObject Boss)
    {
        Vector3 spawnPos = transform.position;

        // 👇 뒤를 보게 회전 설정
        Quaternion rot = Quaternion.Euler(0, 180, 0);

        GameObject enemy = Instantiate(Boss, spawnPos, rot, gameObject.transform);
        enemy.transform.localScale = new Vector3(16 / 5f, 1, 16 / 5f) * 3;
        tmp.text = $"{++player.UnitCount}";
        return enemy.GetComponent<EnemyStats>();
    }

    public IEnumerator SummonLoop()
    {
        for (int i = 0; i < 35; i++)
        {
            Vector3 spawnPos = transform.position;

            // 👇 뒤를 보게 회전 설정
            Quaternion rot = Quaternion.Euler(0, 180, 0);


            GameObject enemy = Instantiate(characterPrefab, spawnPos, rot, gameObject.transform);
            enemy.transform.localScale = new Vector3(16 / 5f, 1, 16 / 5f);
            enemy.GetComponent<EnemyStats>().SetRound(GameManager.Instance.GetRound());
            tmp.text = $"{++player.UnitCount}";
            yield return new WaitForSeconds(summonInterval);
        }
    }

    public void StopSummon()
    {
        if (summonRoutine != null)
        {
            StopCoroutine(summonRoutine);
            summonRoutine = null;
        }
    }
    
}
