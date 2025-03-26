using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Pool : MonoBehaviour
{
    [Header("Pool")]
    [SerializeField] private GameObject weaponPickup;
    [SerializeField] private GameObject ammoPickup;
    public static Object_Pool instance;
  
    
    [SerializeField] private int poolSize = 10;


    

    //เก็บค่าแบบdictionary<ประเภทของข้อมูล(key),ค่าของข้อมูล> ชื่อdictionary      =ให้เท่ากับการเพิ่มkeyของข้อมูลลงไปพร้อมใส่ค่าข้อมูล
    private Dictionary<GameObject,Queue<GameObject>> poolDictionary 
        = new Dictionary<GameObject, Queue<GameObject>>();

    private void Awake() //ถ้ามีสคิรปต์นี้อยู่ในcomponentอื่นให้ลบออกจาcomponentนั้น
        //Awakeคล้ายๆStart
    {
        if(instance == null)
        {
            instance = this;
        }else
        {
            Destroy(gameObject);
        }
        
        
    }
    private void Start()
    {
        InitializeNewPool(weaponPickup);
        InitializeNewPool(ammoPickup);
    }



    //เรียกใช้เพื่อนับถอยหลังรีเทรินobjtopool
    public void ReturnObject(GameObject objToReturn, float delay = .001f) 
        => StartCoroutine(DelayReturn(delay,objToReturn));
        
   
    private IEnumerator DelayReturn(float delay,GameObject objToReturn) //ตั้งเวลาถอยหลังรีเทรินobjtopool
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(objToReturn);
    }
    private void ReturnToPool(GameObject objToReturn)
    {
        GameObject originalPrefab = objToReturn.GetComponent<PooledObject>().originalPrefab;
        objToReturn.SetActive(false);
        objToReturn.transform.parent = transform; //ให้ตำแหน่งobjอยู่ในobjที่สคริปต์นี้อยู่

        poolDictionary[originalPrefab].Enqueue(objToReturn); //รีเทรินobjไปในdictionaryเพื่อให้Poolไม่ว่าง


    }//returnobjเข้าไปในPool
    private void InitializeNewPool(GameObject prefab) //สร้างPoolใหม่
    {
        poolDictionary[prefab] = new Queue<GameObject>(); //เพิ่มPrefabนี้ลงในPool
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject(prefab); //สร้างobjลงในPool
        }
    }

    private void CreateNewObject(GameObject prefab)
    {
        GameObject newObj = Instantiate(prefab, transform);

        //เพิ่มcomponentลงไปในobjที่พึ่งสร้างขึ้นเพื่อให้ในcomponentของobjนั้นเก็บตัวแหน่งoriginalของprefab
        //เพื่อที่เราจะได้ใช้เป็นคีย์ในdictionary
        newObj.AddComponent<PooledObject>().originalPrefab = prefab;
        newObj.SetActive(false);
        poolDictionary[prefab].Enqueue(newObj); //เพิ่มเข้าไปDictionary
    }

    public GameObject GetObject(GameObject prefab,Transform target) //เรียกใช้GameObjในpool
    {
        //เช็คว่าGameobjที่ส่งเข้ามามีตรงกับkeyในของDictionaryมั้ย
        //ถ้าไม่ให้สร้างPoolขึ้นมาใหม่
        if(poolDictionary.ContainsKey(prefab) == false)
        {
            InitializeNewPool(prefab);
        }
        if(poolDictionary[prefab].Count == 0) //ถ้าในdictionaryนั่นไม่มีvalueเลยให้สร้างobjในpoolนั้นและเพิ่มvalueเข้าdictionary
        {
            CreateNewObject(prefab);
        }

        GameObject objToGet = poolDictionary[prefab].Dequeue();
        objToGet.transform.position = target.position;
        objToGet.transform.parent = null;//ให้ไปอยู่นอกGameobject objectpool
        objToGet.SetActive(true);
        return objToGet;
    }



}
