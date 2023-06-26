//G29 Wheel
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;

//Send data to Python
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace G29Module
{

    /*
    Wheel.lX = steering_wheel x axis 
    Wheel.lY = pedal acceleration
    Wheel.lRz = pedal brake
   */


    class program
    {
        //send data to python
        Thread mThread;
        IPAddress localAdd;
        TcpListener listener;
        public static int X_Axis = 0;
        public static int Y_Axis = 0;
        public static int Z_Axis = 0;
        int[,] Values = new int[1, 3];
        public static float speedParam_ = 0.0f;
        public static float brakeParam_ = 0.0f;




        public static void setValues(LogitechGSDK.DIJOYSTATE2ENGINES Wheel)
        {
            X_Axis = Wheel.lX; //2 * ((Wheel.lX - (xMin)) / (xMax - (xMin))) - 1;
            Y_Axis = Wheel.lY;  //Math.Abs(((Wheel.lY - (yzMin)) / (yzMax - (yzMin))));
            Z_Axis = Wheel.lRz; //Math.Abs(((Wheel.lRz - (yzMin)) / (yzMax - (yzMin))));
        }

        public static int getValueX()
        {
            return X_Axis;
        }
        public static float fixX(int Value)
        {
            float Value1 = 0; 
            Value1 = Math.Min((((float)(Value)) / 32767), 1);
            return Value1;
        }
        public static float fixYZ(int Value)
        {
            float Value1 = 0;
            Value1 = -Math.Min((((float)(Value)) / 32767), 0);
            return Value1;
        }
        public static int getValueY()
        {
            return Y_Axis;
        }

        public static int getValueZ()
        {
            return Z_Axis;
        }


        public static void Main(string[] args)
        {
            //main variables declaration
            bool done = false;
            LogitechGSDK.DIJOYSTATE2ENGINES G29Wheel = new LogitechGSDK.DIJOYSTATE2ENGINES();
            LogitechGSDK.LogiControllerPropertiesData properties = new LogitechGSDK.LogiControllerPropertiesData();

            string connectionIP = "127.0.0.2";
            int connectionPort = 25001;
            TcpClient client;
            bool running;
            int x = 0;
            int y = 0;
            int z = 0;
            string message;
            float xFloat = 0;
            float yFloat = 0;
            float zFloat = 0;


            //start g29
            void Start()
            {
                //not ignoring xinput in this example
                LogitechGSDK.LogiSteeringInitialize(false);
                //Console.WriteLine(LogitechGSDK.LogiIsConnected(0));
                
                /*
                properties.wheelRange = 90;
                properties.forceEnable = true;
                properties.overallGain = 80;
                properties.springGain = 80;
                properties.damperGain = 80;
                properties.allowGameSettings = true;
                properties.combinePedals = false;
                properties.defaultSpringEnabled = true;
                properties.defaultSpringGain = 80;
                */
                properties.allowGameSettings = true;
                properties.combinePedals = false;
                properties.damperGain = 100;
                properties.defaultSpringEnabled = true;
                properties.defaultSpringGain = 100;
                properties.forceEnable = true;
                properties.gameSettingsEnabled = false;
                properties.overallGain = 100;
                properties.springGain = 100;
                properties.wheelRange = 200;
                LogitechGSDK.LogiSetPreferredControllerProperties(properties);

                //send data to python
                ThreadStart ts = new ThreadStart(GetInfo);
                Thread mThread = new Thread(ts);
                mThread.Start();

            }

            void GetInfo()
            {
                IPAddress localAdd = IPAddress.Parse(connectionIP);
                TcpListener listener = new TcpListener(IPAddress.Any, connectionPort);
                listener.Start();

                client = listener.AcceptTcpClient();

                running = true;
                while (running)
                {

                        SendAndReceiveData();
                        
                    
                    System.Threading.Thread.Sleep(10);
                }
                listener.Stop();

            }

            void SendAndReceiveData()
            {
                
                NetworkStream nwStream = client.GetStream();
                byte[] buffer = new byte[client.ReceiveBufferSize];
                x = getValueX();
                xFloat = fixX(x);
                y = getValueY();
                yFloat = fixYZ(y);
                z = getValueZ();
                zFloat = fixYZ(z);
                message = string.Format("{0},{1},{2}*", xFloat, yFloat, zFloat);//don't remove the / after {2}. It will break the code
                //x_value = Wheel.lX.ToString();
                //Values = getValues(G29Wheel);
                //x_value = Values[0, 0].ToString();
                //wheelData = string.Join(",", Values[0, 0], Values[0, 1], Values[0, 2]);
                //---Sending Data to Host----
                byte[] myWriteBuffer = Encoding.ASCII.GetBytes(message); //Converting string to byte data
                nwStream.Write(myWriteBuffer, 0, myWriteBuffer.Length); //Sending the data in Bytes to Python

            }

            /*
            void setValues(LogitechGSDK.DIJOYSTATE2ENGINES Wheel)
                {
                
                X_Axis= Wheel.lX;
                Y_Axis = Wheel.lY;
                Z_Axis = Wheel.lRz;
                
                Values[0, 0] = X_Axis;
                Values[0, 1] = Y_Axis;
                Values[0, 2] = Z_Axis;
                /*
                if (ret == '0')
                {
                    return X_Axis;
                }
                if (ret == '1')
                {
                    return Y_Axis;
                }
                if (ret == '2')
                {
                    return Z_Axis;
                }
                else
                {
                    return 0;
                }
                
            }
*/
        
            void ActivatewheelSpringForce(LogitechGSDK.DIJOYSTATE2ENGINES Wheel, int coef)
                {
                int val = (int)Math.Round(speedParam_ * 70);
                LogitechGSDK.LogiPlaySpringForce(0, 0, 2*val, 2*val);
                

                /*
                if (Wheel.lX < 5 && Wheel.lX > -5)
                    {
                        LogitechGSDK.LogiStopSpringForce(0);
                        //LogitechGSDK.LogiStopConstantForce(0);
                    }
                    if (Wheel.lX >= 500)
                    {
                        LogitechGSDK.LogiPlaySpringForce(0, -15 * coef, 50, 50);
                        //LogitechGSDK.LogiPlayConstantForce(0, 15 * coef);
                    }
                    if (Wheel.lX >= 2000)
                    {
                        LogitechGSDK.LogiPlaySpringForce(0, -25 * coef, 50, 50);
                        //LogitechGSDK.LogiPlayConstantForce(0, 25 * coef);
                }
                    if (Wheel.lX >= 5000)
                    {
                        LogitechGSDK.LogiPlaySpringForce(0, -50 * coef, 50, 50);
                        //LogitechGSDK.LogiPlayConstantForce(0, 50 * coef);
                }
                    if (Wheel.lX <= -500)
                    {
                        LogitechGSDK.LogiPlaySpringForce(0, 15 * coef, 50, 50);
                        //LogitechGSDK.LogiPlayConstantForce(0, -15 * coef);
                }
                    if (Wheel.lX <= -2000)
                    {
                        LogitechGSDK.LogiPlaySpringForce(0, 25 * coef, 50, 50);
                        //LogitechGSDK.LogiPlayConstantForce(0, -25 * coef);
                }
                    if (Wheel.lX <= -5000)
                    {
                        LogitechGSDK.LogiPlaySpringForce(0, 50 * coef, 50, 50);
                        //LogitechGSDK.LogiPlayConstantForce(0, -50 * coef);
                    }
                */

                }

            void wheelSpringForce(LogitechGSDK.DIJOYSTATE2ENGINES Wheel)
                {
                    if (Wheel.lY <= 30000)
                    {
                        ActivatewheelSpringForce(Wheel, 2);
                    }
                    else
                    {
                        ActivatewheelSpringForce(Wheel, 1);
                    }
                }

            //input all the functions
            void Update()
                {
                //All the test functions are called on the first device plugged in(index = 0)
                if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
                    {
                    G29Wheel = LogitechGSDK.LogiGetStateCSharp(0);
                    //speedParam_ = Math.Max(((float)(G29Wheel.lY)) / 32767);
                    speedParam_ = Math.Max(((-(float)(G29Wheel.lY)) / 32767), 0);
                    brakeParam_ = Math.Max((((float)(G29Wheel.lRz)) / 32767), 0);
                    //Console.WriteLine(speedParam_);
                    //speedParam_[0] = max((((float)(m_DIJoyState2Device[index_]->lY)) / 32767), 0);
                    //brakeParam_[0] = max(((-(float)(m_DIJoyState2Device[index_]->lY)) / 32767), 0);
                    LogitechGSDK.LogiPlayLeds(0, speedParam_, 0.1f, 1.0f);
                    ActivatewheelSpringForce(G29Wheel,0);
                    setValues(G29Wheel);
                    
                    x = getValueX();
                    xFloat = fixX(x);
                    y = getValueY();
                    yFloat = fixYZ(y);
                    z = getValueZ();
                    zFloat = fixYZ(z);
                    Console.WriteLine("{0},{1},{2}", xFloat, yFloat, zFloat);

                    if (LogitechGSDK.LogiButtonTriggered(0, 23))
                    {
                        Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                    }


                    //wheelData = string.Join(",", Values[0, 0], Values[0, 1], Values[0, 2]);
                    //Console.WriteLine(Values[0, 0]);
                    //Console.WriteLine(wheelData);



                    //Console.WriteLine("{0},{1},{2}", Values[0, 0], Values[0, 1], Values[0, 2]);


                    // if (LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_SPRING))
                    //  {
                    //LogitechGSDK.LogiStopSpringForce(0);
                    //LogitechGSDK.LogiPlaySpringForce(0, -100, -100, -100);
                    //Console.WriteLine("2");                        
                    //getValues(G29Wheel);
                    //wheelSpringForce(G29Wheel);

                    //  }
                    //   else
                    //   {
                    //autoupdate area
                    //Console.WriteLine(G29Wheel.lX);
                    //LogitechGSDK.LogiPlayConstantForce(0, 50);
                    //LogitechGSDK.LogiPlaySpringForce(0, 50, 50, 50);
                    //Console.WriteLine("1");
                    //getValues(G29Wheel);
                    //wheelSpringForce(G29Wheel);

                    //}
                }
                }

                // Use this for shutdown
                void Stop()
                {
                    LogitechGSDK.LogiSteeringShutdown();
                }


                Start();


            
                while (!done)
                {
                    Update();
                //the main window handler has been found, you can now call any function
                //from the steering wheel sdk
              
                System.Threading.Thread.Sleep(10);
            }

                
        }
            //LogitechGSDK.LogiSteeringInitialize(true);
        }

    }



