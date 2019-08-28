using System;
using System.Collections.Generic;
using Microsoft.Azure;
using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Compute.Fluent.Models;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace azure_sdk_practices_netcore
{
    class AzureVMService {
        
        public void createResourceGroup() {
            Microsoft.Azure.Management.Fluent.IAzure azureCredentials = 
                                new AzureCredentialProvider().LoginAzure();

            Console.WriteLine("Creating resource group...");

            var resourceGroup = azureCredentials.ResourceGroups.Define("sdk-netcore-practices")
                                .WithRegion(Region.EuropeNorth)
                                .Create();
        }

        public IAvailabilitySet createAvailabilitySet() {
            Microsoft.Azure.Management.Fluent.IAzure azureCredentials = 
                                new AzureCredentialProvider().LoginAzure();

            Console.WriteLine("Creating availability set...");

            IAvailabilitySet availabilitySet = azureCredentials.AvailabilitySets.Define("myAVSet")
                                  .WithRegion(Region.EuropeNorth)
                                    .WithExistingResourceGroup("sdk-netcore-practices")
                                    .WithSku(AvailabilitySetSkuTypes.Aligned)
                                    .Create();
            return availabilitySet;
        }

        public IPublicIPAddress createPublicIpAddress() {
            Microsoft.Azure.Management.Fluent.IAzure azureCredentials = 
                                new AzureCredentialProvider().LoginAzure();

            Console.WriteLine("Creating public IP address...");

            IPublicIPAddress publicIpAddress = azureCredentials.PublicIPAddresses.Define("myPublicIP")
                                                                    .WithRegion(Region.EuropeNorth)
                                                                    .WithExistingResourceGroup("sdk-netcore-practices")
                                                                    .WithDynamicIP()
                                                                    .Create();
            return publicIpAddress;
        }

        public INetwork createVirtualNetwork() {
            Microsoft.Azure.Management.Fluent.IAzure azureCredentials = 
                                new AzureCredentialProvider().LoginAzure();

            Console.WriteLine("Creating Virual Network...");
            INetwork network = azureCredentials.Networks.Define("myVNet")
                                                   .WithRegion(Region.EuropeNorth)
                                                   .WithExistingResourceGroup("sdk-netcore-practices")
                                                   .WithAddressSpace("10.0.0.0/16")
                                                   .WithSubnet("mySubnet", "10.0.0.0/24")
                                                   .Create();
            return network;
        }

        public INetworkInterface createNetworkInterface(INetwork newNetwork, IPublicIPAddress publicIPAddress) {
            Microsoft.Azure.Management.Fluent.IAzure azureCredentials = 
                                new AzureCredentialProvider().LoginAzure();

            Console.WriteLine("Creating Network Interface...");

            INetworkInterface networkInterface = azureCredentials.NetworkInterfaces.Define("myNIC")
                                                                     .WithRegion(Region.EuropeNorth)
                                                                     .WithExistingResourceGroup("sdk-netcore-practices")
                                                                     .WithExistingPrimaryNetwork(newNetwork)
                                                                     .WithSubnet("mySubnet")
                                                                     .WithPrimaryPrivateIPAddressDynamic()
                                                                     .WithExistingPrimaryPublicIPAddress(publicIPAddress)
                                                                     .Create();
            return networkInterface;
        }

        public IVirtualMachine createVirtualMachine(INetworkInterface networkInterface, IAvailabilitySet availabilitySet) {
            Microsoft.Azure.Management.Fluent.IAzure azureCredentials = 
                                new AzureCredentialProvider().LoginAzure();

            Console.WriteLine("Creating Virtual Machine...");

            IVirtualMachine virtualMachine = azureCredentials.VirtualMachines.Define("myVMFromSDK")
                                                                  .WithRegion(Region.EuropeNorth)
                                                                  .WithExistingResourceGroup("sdk-netcore-practices")
                                                                  .WithExistingPrimaryNetworkInterface(networkInterface)
                                                                  .WithLatestWindowsImage("MicrosoftWindowsServer", "WindowsServer", "2019-Datacenter")
                                                                  .WithAdminUsername("admin12345")
                                                                  .WithAdminPassword("AdminAdmin12345")
                                                                  .WithComputerName("myVMFromSDK")
                                                                  .WithExistingAvailabilitySet(availabilitySet)
                                                                  .WithSize(VirtualMachineSizeTypes.StandardB2s)
                                                                  .Create();
            return virtualMachine;
        }

        public void StopVirtualMachine(String resourceId) {
            Microsoft.Azure.Management.Fluent.IAzure azureCredentials = 
                                new AzureCredentialProvider().LoginAzure();

            Console.WriteLine("Deallocating Virtual Machine...");

            IVirtualMachine virtualMachine = azureCredentials.VirtualMachines.GetById(resourceId);

            virtualMachine.PowerOff();
            virtualMachine.Deallocate();
        }

        public void StartVirtualMachine(String resourceId) {
            Microsoft.Azure.Management.Fluent.IAzure azureCredentials = 
                    new AzureCredentialProvider().LoginAzure();

            Console.WriteLine("Start Virtual Machine...");

            IVirtualMachine virtualMachine = azureCredentials.VirtualMachines.GetById(resourceId);

            virtualMachine.Start();
        }


       public void resizeVirtualMachine(String resourceId) {
            Microsoft.Azure.Management.Fluent.IAzure azureCredentials = 
                                new AzureCredentialProvider().LoginAzure();

            Console.WriteLine("Resizing Virtual Machine to Standard B1s...");

            IVirtualMachine virtualMachine = azureCredentials.VirtualMachines.GetById(resourceId);

            virtualMachine.Update()
                            .WithSize(VirtualMachineSizeTypes.StandardB1s)
                            .Apply();
        }

        public void AddDataDiskToVirtualMachine(String resourceId) {
            Microsoft.Azure.Management.Fluent.IAzure azureCredentials = 
                                new AzureCredentialProvider().LoginAzure();

            Console.WriteLine("Adding data disk to vm with 2GB and Read/Write allowed...");

            IVirtualMachine virtualMachine = azureCredentials.VirtualMachines.GetById(resourceId);

            virtualMachine.Update()
                            .WithNewDataDisk(2, 0, CachingTypes.ReadWrite)
                            .Apply();
        }
    }
}