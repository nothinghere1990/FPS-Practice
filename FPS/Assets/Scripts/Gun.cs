using UnityEngine;

public class Gun : MonoBehaviour
{
    public bool isAutomatic;
    public float timeBetweenShots = .1f, heatPerShot = 1;
    public GameObject muzzleFlash;
    public int shotDamage;
}
