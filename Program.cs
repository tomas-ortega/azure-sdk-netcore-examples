using System;
using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Network.Fluent;

namespace azure_sdk_practices_netcore
{
    class Program
    {
        static void Main(string[] args)
        {
            //Program.runAzureVmPractices();
            //Program.runAzureCosmosDbPractices();
        }

        static void runAzureVmPractices()
        {
             //var vmPractice = new AzureVMService();
            //String machineResourceId = "VIRTUAL_MACHINE_RESOURCE_ID";

            //vmPractice.createResourceGroup();
            /* IAvailabilitySet availabilitySet = vmPractice.createAvailabilitySet();
            IPublicIPAddress myPublicIPAddress = vmPractice.createPublicIpAddress();
            INetwork myNetwork = vmPractice.createVirtualNetwork();
            INetworkInterface myNetworkInterface = vmPractice.createNetworkInterface(myNetwork, myPublicIPAddress);
            IVirtualMachine myVirtualMachien = vmPractice.createVirtualMachine(myNetworkInterface, availabilitySet);*/

            //vmPractice.StopVirtualMachine(machineResourceId);
            //vmPractice.StartVirtualMachine(machineResourceId);
            //vmPractice.resizeVirtualMachine(machineResourceId);
            //vmPractice.AddDataDiskToVirtualMachine(machineResourceId);
        }

        static void runAzureCosmosDbPractices() {
            new AzureCosmosDBPractices();
        }
    }
}
