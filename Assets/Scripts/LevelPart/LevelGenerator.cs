using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{


    [SerializeField] private List<Transform> levelParts; //originalList
    private List<Transform> currentLevelParts; //newlvPart
    private List<Transform> generatedLevelParts = new List<Transform>();//��ʵ���źlevelpart����������ͧ��������
    [SerializeField] private Transform lastLevelPart;
    [SerializeField] private SnapPoint nextSnapPoint;
    private SnapPoint defaultSnapPoint;


    [SerializeField] private float generationCooldown;
    private float cooldownTimer;
    private bool generationOver;
    private void Start()
    {
        defaultSnapPoint = nextSnapPoint;
        InitializeGeneration();
        
    }
    private void Update()
    {
        if (generationOver)
        {
            return;
        }
        cooldownTimer -= Time.time;
        if (cooldownTimer < 0)
        {
            if (currentLevelParts.Count > 0)
            {
                cooldownTimer = generationCooldown;
                GenerateNextLevelPart();
            }
            else if (generationOver == false)
            {
                FinishGeneration();
            }
        }
    }

    [ContextMenu("Restart Generation")]
    private void InitializeGeneration() //���ҧlevelpart����������ѹ���Ż�ѹ
    {
        nextSnapPoint = defaultSnapPoint;
        generationOver = false;
        currentLevelParts = new List<Transform>(levelParts);
        DestroyOldLevelPart();
    }

    private void DestroyOldLevelPart()
    {
        foreach (Transform t in generatedLevelParts)
        {
            Destroy(t.gameObject);
        }

        generatedLevelParts = new List<Transform>();
    }

    private void FinishGeneration()
    {
        generationOver = true;
        GenerateNextLevelPart();

    }

    [ContextMenu("Create next level part")]
    private void GenerateNextLevelPart()
    {
        Transform newPart = null;
        if (generationOver)
        {
            newPart = Instantiate(lastLevelPart);
        }
        else
        {
            newPart = Instantiate(ChooseRandomPart());
        }
        generatedLevelParts.Add(newPart);
        
        LevelPart levelPartScript = newPart.GetComponent<LevelPart>();


        levelPartScript.SnapAndAlignPartTo(nextSnapPoint);
        if (levelPartScript.InterSectionDetected())
        {
            InitializeGeneration();
            return;
        }
        nextSnapPoint = levelPartScript.GetExitPoint(); //��˹�nextpoint��ExitPoint�ͧpart���
    }
    private Transform ChooseRandomPart()
    {   //����SnapPart����ѧ��������ҧ 
        int randomIndex = Random.Range(0, currentLevelParts.Count);
        Transform choosenPart = currentLevelParts[randomIndex];

        currentLevelParts.RemoveAt(randomIndex);

        return choosenPart;
    }

}
