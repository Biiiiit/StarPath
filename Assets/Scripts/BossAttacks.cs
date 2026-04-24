// using UnityEngine;
// using System.Collections;

// public class BossAttacks : MonoBehaviour
// {
//     [Header("Setup")]
//     public GameObject bulletPrefab;

//     private Transform playerTransform;

//     private float leftBound;
//     private float rightBound;
//     private float timePerRow;
//     [Header("Phase Patterns")]
//     public AttackPattern[] phase2Patterns;

//     private AttackPattern[] currentPatterns;
//     [Header("Attack Timing")]
//     public float attackCooldown = 3f;
//     private bool isAttacking = false;
//     private int currentPhase = 1;

//     private bool running = false;

//     public void Init(Transform player, float left, float right, float rowTime)
//     {
//         playerTransform = player;
//         leftBound = left;
//         rightBound = right;
//         timePerRow = rowTime;
//     }

//     public void StartPhase1()
//     {
//         StopAllCoroutines();
//         currentPhase = 1;
//         StartCoroutine(Phase1Loop());
//     }

//     public void StartPhase2()
//     {
//         currentPatterns = phase2Patterns;
//         StartCoroutine(PhaseTransitionDelay());
//     }

//     IEnumerator PhaseTransitionDelay()
//     {
//         isAttacking = true;
//         yield return new WaitForSeconds(attackCooldown);
//         isAttacking = false;
//     }

//     public void StartPhase3()
//     {
//         StopAllCoroutines();
//         currentPhase = 3;
//         StartCoroutine(Phase3Loop());
//     }

//     IEnumerator Phase1Loop()
//     {
//         while (currentPhase == 1)
//         {
//             yield return Phase1Attack();
//             yield return new WaitForSeconds(3f);
//         }
//     }

//     IEnumerator Phase1Attack()
//     {
//         bool fireRows = Random.value > 0.7f;

//         if (!fireRows)
//         {
//             yield return FireSingleShot();
//         }
//         else
//         {
//             for (int i = 0; i < 4; i++)
//             {
//                 yield return FireSingleShot();
//                 yield return new WaitForSeconds(timePerRow);
//             }
//         }
//     }

//     IEnumerator FireSingleShot()
//     {
//         bool aimed = Random.value > 0.5f;

//         float xPos = aimed
//             ? playerTransform.position.x
//             : Random.Range(leftBound, rightBound);

//         Vector3 spawnPos = new Vector3(xPos, transform.position.y, 1);

//         GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

//         bullet.GetComponent<BossBullet>().SetDirection(Vector2.down);

//         yield return null;
//     }

//     IEnumerator Phase2Loop()
//     {
//         while (currentPhase == 2)
//         {
//             yield return new WaitForSeconds(2f);
//             // Plug your pattern system here later
//         }
//     }

//     IEnumerator Phase3Loop()
//     {
//         while (currentPhase == 3)
//         {
//             int attackType = Random.Range(0, 3);

//             if (attackType == 0)
//                 yield return AimedBurst();
//             else if (attackType == 1)
//                 yield return SpiralAttack();
//             else
//                 yield return RadialBurst();

//             yield return new WaitForSeconds(1f);
//         }
//     }

//     IEnumerator AimedBurst()
//     {
//         int shots = 5;

//         for (int i = 0; i < shots; i++)
//         {
//             float xPos = playerTransform.position.x + Random.Range(-0.5f, 0.5f);

//             Vector3 spawnPos = new Vector3(xPos, transform.position.y, 1);

//             Vector2 dir = Vector2.down;

//             for (int j = 0; j < 3; j++)
//             {
//                 GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
//                 bullet.GetComponent<BossBullet>().SetDirection(dir);
//             }

//             yield return new WaitForSeconds(0.1f);
//         }
//     }

//     IEnumerator SpiralAttack()
//     {
//         int bullets = 20;
//         float angleStep = 20f;
//         float currentAngle = Random.Range(0f, 360f);

//         Vector3 center = new Vector3(transform.position.x, transform.position.y, 1);

//         for (int i = 0; i < bullets; i++)
//         {
//             float angle = currentAngle + i * angleStep;
//             float rad = angle * Mathf.Deg2Rad;

//             Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

//             GameObject bullet = Instantiate(bulletPrefab, center, Quaternion.identity);
//             bullet.GetComponent<BossBullet>().SetDirection(dir);

//             yield return new WaitForSeconds(0.05f);
//         }
//     }

//     IEnumerator RadialBurst()
//     {
//         int bullets = 24;

//         Vector3 center = new Vector3(transform.position.x, transform.position.y, 1);

//         for (int i = 0; i < bullets; i++)
//         {
//             float angle = (360f / bullets) * i;
//             float rad = angle * Mathf.Deg2Rad;

//             Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

//             GameObject bullet = Instantiate(bulletPrefab, center, Quaternion.identity);
//             bullet.GetComponent<BossBullet>().SetDirection(dir);
//         }

//         yield return new WaitForSeconds(0.3f);
//     }
// }