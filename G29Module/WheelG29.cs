/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G29Module
{
    class WheelG29
    {
        public WheelG29() {
         try
            {
                
            // Find all the GameControl devices that are attached.
            DeviceList gameControllerList = Manager.GetDevices(DeviceClass.GameControl,
                                                                EnumDevicesFlags.AttachedOnly);

            // Verify if there are gamecontroller attached
            if (gameControllerList.Count > 0)
            {

                foreach (DeviceInstance devInst in gameControllerList)
                {
                    if (devInst.DeviceType == DeviceType.Driving && devInst.ProductName.Contains("Logitech G27 Racing Wheel USB"))
                    {
                        Wheel = new Device(devInst.InstanceGuid);
        Wheel.SetCooperativeLevel(ParentControl, CooperativeLevelFlags.Background | CooperativeLevelFlags.Exclusive);
                        Wheel.SetDataFormat(DeviceDataFormat.Joystick);
                        Wheel.Properties.AxisModeAbsolute = true;
                        Wheel.Properties.AutoCenter = false;
                        Wheel.Acquire();                        
                    }
}
            }

            if (Wheel == null)
{
    throw new NotImplementedException();
}

        }

        catch (Exception Ex)
{
    MessageBox.Show("WeelG27() Constructor error!\n" + Ex.Message + "\n" + Ex.StackTrace);
}
            }
        }
}
} }
*/