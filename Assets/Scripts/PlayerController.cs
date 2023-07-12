using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject FreezeEffect;
    [SerializeField] private GameObject FreezeIcon;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Image crossHair;

    [SerializeField] private Transform pinPos;

    private GameObject summonedEffect;
    private Transform selectObject;
    private Rigidbody selRigid;

    private bool freezeSkillAble = true;
    private Rigidbody freezeObject;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("End"))
        {
            gameManager.GameClear();
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q) && freezeSkillAble && selectObject != null)
        {
            StartCoroutine(FreezeSkillDelay());
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (selectObject == null) CheckingObject();
            else
            {
                selRigid.velocity = Vector3.zero;
                ChangeLayersRecursively(selectObject, "Ground");
                selRigid.constraints = RigidbodyConstraints.None;
                selRigid = null;
                selectObject = null;
                crossHair.color = Color.white;
            }
        }
        if(Input.GetMouseButtonDown(1) && selectObject != null)
        {
            StandObject();
        }
    }

    IEnumerator FreezeSkillDelay()
    {
        crossHair.color = Color.white;
        summonedEffect = Instantiate(FreezeEffect, selectObject);
        FreezeIcon.SetActive(false);
        freezeSkillAble = false;
        freezeObject = selRigid;
        freezeObject.constraints = RigidbodyConstraints.FreezeAll;
        selRigid.velocity = Vector3.zero;
        ChangeLayersRecursively(selectObject, "Ground");
        selRigid = null;
        selectObject = null;
        yield return new WaitForSeconds(60f);
        if (summonedEffect != null) { Destroy(summonedEffect); }
        freezeObject.constraints = RigidbodyConstraints.None;
        yield return new WaitForSeconds(30f);
        freezeSkillAble = true;
        FreezeIcon.SetActive(true);
    }

    private void FixedUpdate()
    {
        if (selectObject != null)
        {
            PinObject();
        }
    }

    private void PinObject()
    {
        selRigid.velocity = Vector3.zero;
        selRigid.AddForceAtPosition((pinPos.position - selectObject.position) * 5, selectObject.position, ForceMode.VelocityChange);
    }
    
    private void StandObject()
    {
        selectObject.DOLocalRotateQuaternion(Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y + 180, 0), 0.3f).SetEase(Ease.InOutQuad);
    }

    private void CheckingObject()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        int layerMask = 1 << LayerMask.NameToLayer("Player");

        if (Physics.Raycast(ray, out hit, 2f, ~layerMask))
        {
            if (hit.transform.CompareTag("InteractObject"))
            {
                selectObject = hit.transform;
                selRigid = selectObject.GetComponent<Rigidbody>();
                selRigid.velocity = Vector3.zero;
                selRigid.constraints = RigidbodyConstraints.FreezeRotation;
                ChangeLayersRecursively(selectObject, "SelectObject");
                crossHair.color = Color.magenta;
                if(freezeObject != null && selectObject.gameObject == freezeObject.gameObject)
                {
                    Destroy(summonedEffect);
                    summonedEffect = null;
                }
            }
        }
    }

    public void ChangeLayersRecursively(Transform trans, string name)
    {
        trans.gameObject.layer = LayerMask.NameToLayer(name);
        foreach (Transform child in trans)
        {
            ChangeLayersRecursively(child, name);
        }
    }
}
