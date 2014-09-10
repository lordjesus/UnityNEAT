using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;

public class CarController : UnitController {

    public float Speed = 5f;
    public float TurnSpeed = 180f;
    public int Lap = 1;
    public int CurrentPiece, LastPiece;
    bool MovingForward = true;
    bool IsRunning;
    public float SensorRange = 10;
    int WallHits; 
    IBlackBox box;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    void FixedUpdate()
    {
        //grab the input axes
        //var steer = Input.GetAxis("Horizontal");
        //var gas = Input.GetAxis("Vertical");

        ////if they're hittin' the gas...
        //if (gas != 0)
        //{
        //    //take the throttle level (with keyboard, generally +1 if up, -1 if down)
        //    //  and multiply by speed and the timestep to get the distance moved this frame
        //    var moveDist = gas * speed * Time.deltaTime;

        //    //now the turn amount, similar drill, just turnSpeed instead of speed
        //    //   we multiply in gas as well, which properly reverses the steering when going 
        //    //   backwards, and scales the turn amount with the speed
        //    var turnAngle = steer * turnSpeed * Time.deltaTime * gas;

        //    //now apply 'em, starting with the turn           
        //    transform.Rotate(0, turnAngle, 0);

        //    //and now move forward by moveVect
        //    transform.Translate(Vector3.forward * moveDist);
        //}

        // Five sensors: Front, left front, left, right front, right

        if (IsRunning)
        {
            float frontSensor = 0;
            float leftFrontSensor = 0;
            float leftSensor = 0;
            float rightFrontSensor = 0;
            float rightSensor = 0;
            // Front sensor
            RaycastHit hit;
            if (Physics.Raycast(transform.position + transform.forward * 1.1f, transform.TransformDirection(new Vector3(0, 0, 1).normalized), out hit, SensorRange))
            {
                if (hit.collider.tag.Equals("Wall"))
                {
                    frontSensor = 1 - hit.distance / SensorRange;
                }
            }

            if (Physics.Raycast(transform.position + transform.forward * 1.1f, transform.TransformDirection(new Vector3(0.5f, 0, 1).normalized), out hit, SensorRange))
            {
                if (hit.collider.tag.Equals("Wall"))
                {
                    rightFrontSensor = 1 - hit.distance / SensorRange;
                }
            }

            if (Physics.Raycast(transform.position + transform.forward * 1.1f, transform.TransformDirection(new Vector3(1, 0, 0).normalized), out hit, SensorRange))
            {
                if (hit.collider.tag.Equals("Wall"))
                {
                    rightSensor = 1 - hit.distance / SensorRange;
                }
            }

            if (Physics.Raycast(transform.position + transform.forward * 1.1f, transform.TransformDirection(new Vector3(-0.5f, 0, 1).normalized), out hit, SensorRange))
            {
                if (hit.collider.tag.Equals("Wall"))
                {
                    leftFrontSensor = 1 - hit.distance / SensorRange;
                }
            }

            if (Physics.Raycast(transform.position + transform.forward * 1.1f, transform.TransformDirection(new Vector3(-1, 0, 0).normalized), out hit, SensorRange))
            {
                if (hit.collider.tag.Equals("Wall"))
                {
                    leftSensor = 1 - hit.distance / SensorRange;
                }
            }

            ISignalArray inputArr = box.InputSignalArray;
            inputArr[0] = frontSensor;
            inputArr[1] = leftFrontSensor;
            inputArr[2] = leftSensor;
            inputArr[3] = rightFrontSensor;
            inputArr[4] = rightSensor;

            box.Activate();

            ISignalArray outputArr = box.OutputSignalArray;

            var steer = (float)outputArr[0] * 2 - 1;
            var gas = (float)outputArr[1] * 2 - 1;

            var moveDist = gas * Speed * Time.deltaTime;
            var turnAngle = steer * TurnSpeed * Time.deltaTime * gas;

            transform.Rotate(new Vector3(0, turnAngle, 0));
            transform.Translate(Vector3.forward * moveDist);
        }
    }

    public override void Stop()
    {
        this.IsRunning = false;
    }

    public override void Activate(IBlackBox box)
    {
        this.box = box;
        this.IsRunning = true;
    }

    public void NewLap()
    {        
        if (LastPiece > 2 && MovingForward)
        {
            Lap++;            
        }
    }

    public override float GetFitness()
    {
        if (Lap == 1 && CurrentPiece == 0)
        {
            return 0;
        }
        int piece = CurrentPiece;
        if (CurrentPiece == 0)
        {
            piece = 17;
        }
        float fit = Lap * piece - WallHits * 0.2f;
      //  print(string.Format("Piece: {0}, Lap: {1}, Fitness: {2}", piece, Lap, fit));
        if (fit > 0)
        {
            return fit;
        }
        return 0;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag.Equals("Road"))
        {
            RoadPiece rp = collision.collider.GetComponent<RoadPiece>();
          //  print(collision.collider.tag + " " + rp.PieceNumber);
            
            if ((rp.PieceNumber != LastPiece) && (rp.PieceNumber == CurrentPiece + 1 || (MovingForward && rp.PieceNumber == 0)))
            {
                LastPiece = CurrentPiece;
                CurrentPiece = rp.PieceNumber;
                MovingForward = true;                
            }
            else
            {
                MovingForward = false;
            }
            if (rp.PieceNumber == 0)
            {
                CurrentPiece = 0;
            }
        }
        else if (collision.collider.tag.Equals("Wall"))
        {
            WallHits++;
        }
    }



    //void OnGUI()
    //{
    //    GUI.Button(new Rect(10, 200, 100, 100), "Forward: " + MovingForward + "\nPiece: " + CurrentPiece + "\nLast: " + LastPiece + "\nLap: " + Lap);
    //}
    
}
