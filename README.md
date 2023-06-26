# LogitechG29_Controller_Carla
Integration of G29 Wheel Module for Real-time Data Retrieval and Transmission to Python

This project is focused on developing a G29 Wheel module that enables real-time data retrieval from the Logitech G29 steering wheel and seamless transmission of the collected data to a Python script for further analysis and processing. Using C# and LogitechGSDK, the module effectively interacted with the G29 steering wheel, capturing crucial data such as the X-axis steering, pedal acceleration, and pedal brake values.

The module implemented a TCP client-server communication approach to establish a connection between the C# module and the Python script. This allowed for smooth integration and data transfer, ensuring efficient collaboration between the two programming languages. The retrieved G29 wheel data was preprocessed within the C# module, including fixing and normalizing the X-axis steering values to ensure consistent and accurate data representation.

The processed data was then transmitted as a string to the Python script, providing the Python environment with access to real-time G29 wheel data for further analysis and utilization. This integration of C# and Python facilitated advanced data analysis, machine learning algorithms, or any other processing techniques available in the Python ecosystem.
