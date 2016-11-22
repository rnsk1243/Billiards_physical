using UnityEngine;
using System.Collections;

public class BallEnergy : MonoBehaviour {

    Rigidbody rig;
    Transform tr;
   // public float MOElasticity = 0.0000001f;
    public float MOElasticity = 0.999999999f;
    public bool isOK = false;
    BallEnergy OtherBall;
    Vector3 v1_f = Vector3.zero;
    // Use this for initialization
    void Start () {
        rig = GetComponent<Rigidbody>();
        //rig.velocity = Vector3.zero;
        tr = GetComponent<Transform>();
    }
	
	// Update is called once per frame
	void Update () {
        // Debug.Log(this.name + " : " + rig.velocity);
        // 출발 준비 됐나?(계산 완료 되었나?)
            if (isOK)
            {
               // Debug.Log(this.name + "v1_f : " + v1_f); // 충돌 후 자신의 속도 출력용
               // 충돌 후 자신의 속도 
                BallStart(v1_f);
                isOK = false;
            }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Right")
        {
           // Debug.Log("right");
            //Debug.Log(rig.velocity);
            rig.velocity = new Vector3(-rig.velocity.x, 0.0f, rig.velocity.z);
        }
        else if (coll.gameObject.tag == "Left")
        {
          //  Debug.Log("Left");
            //Debug.Log("전 = " + rig.velocity);
            // rig.velocity = Vector3.zero;
            rig.velocity = new Vector3(-rig.velocity.x, 0.0f, rig.velocity.z);
            // Debug.Log("후 = " + rig.velocity);
        }
        else if (coll.gameObject.tag == "Up")
        {
            rig.velocity = new Vector3(rig.velocity.x, 0.0f, -rig.velocity.z);
           // Debug.Log("Up");
        }
        else if (coll.gameObject.tag == "Bottom")
        {
            rig.velocity = new Vector3(rig.velocity.x, 0.0f, -rig.velocity.z);
           // Debug.Log("Bottom");
        }
        else if (coll.gameObject.tag == "Ball")
        {
          //  Debug.Log("공과 충돌함");

            /*
            modulus of elasticity <= 탄성계수
             m1, m2 는 각각 공의 질량
             v1_i, v2_i 는 각각 공의 충돌전 속도
             v1_f, v2_f 는 각각 공의 충돌후 속도
            =================================================================================

             운동량 보존의 법칙에 따라 m1v1_i +m2v2_i = m1v1_f + m2v2_f 가 성립 함-- 1번 식
             (v1_f - v2_f) = -탄성계수(v1_i - v2_i)------------------------------ - 2번 식
             
            위 공식을 이용하여 충돌후 v1_f 와 v2_f를 구함
            
            우선 m1v1_i + m2v2_i = m1v1_f + m2v2_f 을 이용해 본다.
            (전제1) 당구공의 질량은 똑같다 즉, m1 == m2 가 성립함
            따라서 v1_i + v2_i = v1_f + v2_f 이다. ---------------------------------- 1번 식
            
            1번식과 2번식을 더하면 v1_f와 v2_f를 구할 수 있다.
             v1_f + v2_f = v1_i + v2_i  ------------------------------------------- 1번 식
           +)(v1_f - v2_f) = -탄성계수(v1_i - v2_i)-------------------------------- 2번 식
            ----------------------------------------
            2v1_f = (v1_i + v2_i) - 탄성계수(v1_i - v2_i)
            v1_f = ((v1_i + v2_i) - 탄성계수(v1_i - v2_i)) / 2 -------------------- v1_f값
            
            v2_f = v1_i + v2_i - v1_f
            v2_f = v1_i + v2_i - ((v1_i + v2_i) - 탄성계수(v1_i - v2_i)) / 2 ------ v2_f값
 
 *     */
            // 충돌하는 물체 처음속도 v1_i // 나중속도 v1_f  각각 x,z 성분 따로 구함.
            // 충돌당할 물체 처음속도 v2_i // 나중속도 v2_f
            //Debug.Log("나의 이름은 : " + this.name);
            //if (coll.gameObject.name == "Ball (3)")
            //{
            //    Debug.Log("나는 꺼져..");
            //    return;
            //}
            BallEnergy OtherBall = coll.gameObject.GetComponent<BallEnergy>();

            float v1_i_x = this.rig.velocity.x;

            float v1_i_z = this.rig.velocity.z;
            //Debug.Log("name = " + coll.gameObject.name);
            //Debug.Log("v1_i_x = " + v1_i_x);
            //Debug.Log("v1_i_z = " + v1_i_z);
            //Debug.Log("OtherBall = " + OtherBall);
            //Debug.Log("OtherBall.rig.velocity.x = " + OtherBall.rig.velocity.x);

            Vector3 OtherBallVelocity = OtherBall.getVelocity();
            //Debug.Log(this.name + "의 OtherBallVelocity : " + OtherBallVelocity);
            //Debug.Log(this.name + "의 coll = " + coll.name);

            // 충돌전 충돌당할 물체의 속도를 가져옴
            float v2_i_x = OtherBallVelocity.x;
            float v2_i_z = OtherBallVelocity.z;
            //Debug.Log("v2_i_x = " + v2_i_x);
            //Debug.Log("v2_i_z = " + v2_i_z);

            // v1_f = ((v1_i + v2_i) - 탄성계수(v1_i - v2_i)) / 2 -------------------- v1_f값
            float v1_f_x = get_V1_F(v1_i_x, v2_i_x);
            float v1_f_z = get_V1_F(v1_i_z, v2_i_z);

            //Debug.Log("v1_f_x = " + v1_f_x);
            //Debug.Log("v1_f_z = " + v1_f_z);

            // 충돌할때 Ball 끼리 충돌하므로 서로 자신의 OnTriggerEnter를 호출 한다.
            // 따라서 이곳에서 충돌하는 모든 Ball의 나중속도를 구해 버리면
            // 각각 Ball이 한번씩 모두 2번 나중속도를 구하는 꼴이 되버린다.
            // 이렇게 되면 충돌 당하는 Ball이 순간적으로 다시 충돌 하는 Ball에게 속도를 줘버려서
            // 마치 아무일도 없는냥 충돌 하는 Ball이 그냥 통과하는 것 처럼 보인다.

            // 따라서 나 외에 다른 Ball은 신경 쓰지 말고 각자 자신의 Ball의 속도를 구하기로 했다.
            // 그래서 v2_f를 구하는 곳은 모두 주석 처리 했다.

            // 하지만 각각 공의 속도를 구하려고 하니.. 문제가 있었다.
            // 충돌 하는 쪽과 충돌 당하는 쪽, 둘 중에 먼저 호출 된 OnTriggerEnter함수가 자신의 
            // 속도를 바꾸어 버리는 바람에 나중에 호출 된 Ball의 OnTriggerEnter함수는 
            // 먼저 호출되어 이미 속도가 바뀌어 버린 v2_i를 가져와 버린다...
            // 이 문제를 해결하기 위해 준비가 되었냐는 bool isOK변수를 만들었다.
            // 충돌하는 두 공이 모두 자신의 나중 속도를 구한 후에 isOK = true로 변경해주었다.
            // 그리고 Update함수에서 isOK가 true일때 자신의 나중 속도를 적용해 주었다.

            // v2_f = v1_i + v2_i - v1_f -------------------------------- v2_f값
            //float v2_f_x = v1_i_x + v2_i_x - v1_f_x;
            //float v2_f_z = v1_i_z + v2_i_z - v1_f_z;

            // 속도 벡터 만듬
            v1_f = new Vector3(v1_f_x, 0.0f, v1_f_z);
            //Vector3 v2_f = new Vector3(v2_f_x, 0.0f, v2_f_z);

            // 출발!
            //Debug.Log("v2_f = " + v2_f);
            //Debug.Log("v1_i_x = " + v1_i_x);
            //Debug.Log("v1_f_x = " + v1_f_x);
            //OtherBall.BallStart(v2_f);
            //Debug.Log(this.name + " v1_f : " + v1_f);
            //Debug.Log(this.name + " v2_f : " + v2_f);

            isOK = true;
            //Debug.Log(this.name + " isOK = " + isOK);
            return;
        }
    }

    //void OnCollisionEnter(Collision coll)
    //{
       
    //}

    public void BallStart(Vector3 vec3)
    {
       
        rig.velocity = vec3;
    }
    // 피타고라스정리 - 힘의 크기를 스칼라값으로 구함. 
    public float Pythagoras_theorem(float a, float b)
    {
        float c = Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2));
        return c;
    }
    // 각각의 처음속도를 넣어 v1_f 구해주는 함수
    public float get_V1_F(float v1_i, float v2_i)
    {
        // v1_f = ((v1_i + v2_i) - 탄성계수(v1_i - v2_i)) / 2 -------------------- v1_f값
        float v1_f = ((v1_i + v2_i) - (MOElasticity * (v1_i - v2_i))) / 2;
        //Debug.Log("v1_f ==== " + v1_f);
        return v1_f;
    }

    public Vector3 getVelocity()
    {
       // Debug.Log(rig.velocity);
        return rig.velocity;
    }
}
