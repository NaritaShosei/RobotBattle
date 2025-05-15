using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LockOn : MonoBehaviour
{
    [SerializeField]
    Transform _player;

    [SerializeField]
    Transform _bulletParent;

    [SerializeField, Header("0～1の間 Debug Only")]
    Vector2 _lockOnCenterScreenPos = new Vector2(0.5f, 0.55f);

    [SerializeField]
    float _maxDistance;

    [SerializeField, Range(0, 180)]
    float _viewAngle = 60;

    Camera _camera;

    IEnemy _lockOnEnemy;

    CrosshairPresenter _presenter;
    void Start()
    {
        _camera = Camera.main;
        _presenter = new CrosshairPresenter(GameUIManager.Instance.CrosshairView);
        _presenter.Initialize();
    }

    void Update()
    {
        float bestScore = float.MinValue;
        _lockOnEnemy = null;
        //EnemyListをEnemyManagerから参照する方針に変更する
        foreach (var enemy in EnemyManager.Instance.Enemies.Where(x => x.IsTargetInView()))
        {
            Vector3 enemyPos = enemy.GetTargetCenter().position;

            //距離チェック
            Vector3 dirToEnemy = enemyPos - _camera.transform.position;
            if (dirToEnemy.magnitude > _maxDistance) continue;


            //視野角チェック
            float angleToEnemy = Vector3.Angle(_camera.transform.forward, dirToEnemy);
            if (angleToEnemy > _viewAngle * 0.5f) continue;


            //実際に見えているかチェック
            if (!IsVisible(enemy)) continue;

            //指定ポイントからの距離計算
            Vector3 viewportPosition = _camera.WorldToViewportPoint(enemyPos);
            Vector2 screenPos = new Vector2(viewportPosition.x, viewportPosition.y);
            float disToCenter = Vector2.Distance(screenPos, _lockOnCenterScreenPos);

            //Y成分を無視したPlayerとEnemyの距離
            float playerDis = (new Vector3(_player.transform.position.x, 0, _player.transform.position.z)
                                                                        - new Vector3(enemyPos.x, 0, enemyPos.z)).magnitude;

            //スコア計算
            //指定ポイントからの距離とY成分を無視したPlayerとEnemyの距離のスコア倍率
            float centerValue = 0.5f;
            float playerValue = 1.5f;
            //0で割らないように小さい数を足す
            float score = (1 / (disToCenter + 0.001f)) * centerValue +
                          (1 / (playerDis + 0.001f)) * playerValue;

            //プレイヤーからの近さをより優先しつつ、画面中央への近さも考慮する
            if (score > bestScore)
            {
                bestScore = score;
                _lockOnEnemy = enemy;
            }

        }
        if (_lockOnEnemy != null)
        {
            _presenter.UpdateLockOn(true, _lockOnEnemy.GetTargetCenter().position);
        }
        else
        {
            _presenter.UpdateLockOn(false, Vector3.zero);
        }
    }

    bool IsVisible(IEnemy enemy)
    {
        //方向、距離計算
        var dirToEnemy = enemy.GetTargetCenter().position - _camera.transform.position;
        float disToEnemy = dirToEnemy.magnitude;

        //カメラを始点にレイキャストを飛ばす
        var hits = Physics.RaycastAll(_camera.transform.position, dirToEnemy.normalized, disToEnemy);
        foreach (var hit in hits)
        {
            //特定のコライダーの場合は無視
            if (hit.collider.CompareTag("IgnoreCollider")) continue;

            //子オブジェクトを含めPlayerなら無視
            if (hit.transform == _player || hit.transform.IsChildOf(_player)) continue;

            //子オブジェクトを含めEnemyなら無視
            if (hit.transform == enemy.GetTargetCenter() || hit.transform.IsChildOf(enemy.GetTransform())) continue;

            //子オブジェクトを含めBulletなら無視
            if (hit.transform == _bulletParent || hit.transform.IsChildOf(_bulletParent)) continue;

            //それ以外でEnemyより手前ならfalse
            if (hit.distance < disToEnemy)
            {
                return false;
            }
        }

        return true;
    }
    public IEnemy GetTarget()
    {
        return _lockOnEnemy;
    }

    public Vector2 GetCrosshairPos()
    {
        return _presenter.GetCrosshairPos();
    }

    void OnDrawGizmos()
    {
        if (_camera == null) _camera = Camera.main;
        if (_camera == null) return;

        Gizmos.color = Color.yellow;
        Vector3 cameraPos = _camera.transform.position;
        Vector3 forward = _camera.transform.forward;

        // 視野角の可視化
        float halfAngle = _viewAngle * 0.5f * Mathf.Deg2Rad;
        Vector3 right = _camera.transform.right;
        Vector3 up = _camera.transform.up;

        // 視野角の端を示す方向ベクトル
        Vector3 forwardRight = Quaternion.AngleAxis(_viewAngle * 0.5f, up) * forward;
        Vector3 forwardLeft = Quaternion.AngleAxis(-_viewAngle * 0.5f, up) * forward;

        // 視野角の線を描画
        Gizmos.DrawRay(cameraPos, forwardRight * _maxDistance);
        Gizmos.DrawRay(cameraPos, forwardLeft * _maxDistance);

        // 視野範囲の円弧を描画
        int segments = 20;
        Vector3 prevPos = cameraPos + forwardRight * _maxDistance;

        for (int i = 1; i <= segments; i++)
        {
            float angle = _viewAngle * ((float)i / segments - 0.5f);
            Vector3 direction = Quaternion.AngleAxis(angle, up) * forward;
            Vector3 pos = cameraPos + direction * _maxDistance;
            Gizmos.DrawLine(prevPos, pos);
            prevPos = pos;
        }
    }
}
