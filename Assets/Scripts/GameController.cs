using System;
using System.Collections;
using System.Linq;
using UnityEngine;
//Придумайте механику, которая работает на корутине
//    а.) Период выстрела оружия 
//    б.) Ожидание окончания хода
//    в.) Перемещение или анимация объекта
//    г.) Ваш вариант
public class GameController : MonoBehaviour
{
    [SerializeField] private CharacterComponent[] playerCharacters;
    [SerializeField] private CharacterComponent[] enemyCharacters;

    private Coroutine gameLoop;
    private bool waitingForInput;
    private CharacterComponent currentTarget;
    public bool IsMoving = false;
    public Vector3 targetPos;

    private void Start()
    {
        waitingForInput = true;
        gameLoop = StartCoroutine(GameLoop());
       // StartCoroutine(HomeWorCorutine());
       // StartCoroutine(MoveCoroutine(targetPos));
    }

    private IEnumerator HomeWorCorutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            Debug.Log(Time.deltaTime);
        }
    }

    IEnumerator MoveCoroutine(Vector3 moveTo)
    {
        IsMoving = true;

        // делаем переход от текущей позиции к новой
        var iniPosition = transform.position;
        while (transform.position != moveTo)
        {
            float MoveY = 0;
            MoveY++;
            gameObject.transform.Translate(0, MoveY, 0);
            // и ждем следующего фрейма
            yield return new WaitForSecondsRealtime(0.00001f);
        }

        IsMoving = false;
    }

    private IEnumerator GameLoop()
    {
        Coroutine turn = StartCoroutine(Turn(playerCharacters, enemyCharacters));

        yield return new WaitUntil(() =>
        playerCharacters.FirstOrDefault(c => !c.HealthComponent.IsDead) == null ||
        enemyCharacters.FirstOrDefault(c => !c.HealthComponent.IsDead) == null);

        StopCoroutine(turn);
        GameOver();
    }

    private CharacterComponent GetTarget(CharacterComponent[] characterComponents)
    {
        var characterComponent = characterComponents.FirstOrDefault(c => !c.HealthComponent.IsDead);
        if (characterComponent != null)
        {
            characterComponent.IndicatorComponent.EnableTargetIndicator();
        }

        return characterComponent;
    }

    private void GameOver()
    {
        bool isPlayerCharacherAlive = false;
        bool isEnemyCharacherAlive = false;

        bool isVictory;

        for (int i = 0; i < playerCharacters.Length; i++)
        {
            if (!playerCharacters[i].HealthComponent.IsDead)
            {
                isPlayerCharacherAlive = true;
            }
        }

        for (int i = 0; i < enemyCharacters.Length; i++)
        {
            if (!enemyCharacters[i].HealthComponent.IsDead)
            {
                isEnemyCharacherAlive = true;
            }
        }

        isVictory = isPlayerCharacherAlive && !isEnemyCharacherAlive;

        Debug.Log(isVictory ? "Victory" : "Defeat");
    }

    private IEnumerator Turn(CharacterComponent[] playerCharacters, CharacterComponent[]
        enemyCharacters)
    {
        int turnCounter = 0;
        while (true)
        {
            foreach (var player in playerCharacters)
            {
                if (currentTarget == null)
                {
                    currentTarget = GetTarget(enemyCharacters);
                }

                while (waitingForInput)
                {
                    yield return null;
                }

                if (player.HealthComponent.IsDead)
                {
                    Debug.Log("Character: " + player.name + " is dead");
                    continue;
                }

                player.SetTarget(currentTarget.HealthComponent);

                //TODO: hotfix
                yield return null; // ugly fix need to investigate
                player.StartTurn();
                yield return new WaitUntilCharacterAttack(player);

                Debug.Log("Character: " + player.name + " finished turn");
                waitingForInput = true;
                currentTarget.IndicatorComponent.DisableTargetIndicator();
                currentTarget = null;
            }

            yield return new WaitForSeconds(.5f);
            foreach (var enemy in enemyCharacters)
            {
                if (enemy.HealthComponent.IsDead)
                {
                    Debug.Log("Enemy character: " + enemy.name + " is dead");
                    continue;
                }

                var characterComponent = GetTarget(playerCharacters);
                enemy.SetTarget(characterComponent.HealthComponent);
                enemy.StartTurn();

                yield return new WaitUntilCharacterAttack(enemy);
                Debug.Log("Enemy character: " + enemy.name + " finished turn");
                characterComponent.IndicatorComponent.DisableTargetIndicator();
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            waitingForInput = false;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            NextTarget();
        }
    }

    public void AttackBtn()
    {
        waitingForInput = false;
    }

    public void SwitchBtn()
    {
        NextTarget();
    }

    private void NextTarget()
    {
        int index = Array.IndexOf(enemyCharacters, currentTarget);
        for (int i = 1; i < enemyCharacters.Length; i++)
        {
            int next = (index + i) % enemyCharacters.Length;
            if (!enemyCharacters[next].HealthComponent.IsDead)
            {
                currentTarget.IndicatorComponent.DisableTargetIndicator();
                currentTarget = enemyCharacters[next];
                currentTarget.IndicatorComponent.EnableTargetIndicator();
                return;
            }
        }
    }

}
