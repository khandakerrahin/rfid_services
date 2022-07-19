using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL950_console
{
    class ErrorCodeDefinitions
    {
        public static Dictionary<int, String> map = new Dictionary<int, string>();
        public static void populate()
        {
            ErrorCodeDefinitions.map[0x01] = "Return before Inventory finished";
            ErrorCodeDefinitions.map[0x02] = "the Inventory-scan-time overflow";
            ErrorCodeDefinitions.map[0x03] = "More Data";
            ErrorCodeDefinitions.map[0x04] = "Reader module MCU is Full";
            ErrorCodeDefinitions.map[0x05] = "Access password error";
            ErrorCodeDefinitions.map[0x09] = "Destroy password error";
            ErrorCodeDefinitions.map[0x0a] = "Destroy password error cann’t be Zero";
            ErrorCodeDefinitions.map[0x0b] = "Tag Not Support the command";
            ErrorCodeDefinitions.map[0x0c] = "Use the commmand,Access Password Cann’t be Zero";
            ErrorCodeDefinitions.map[0x0d] = "Tag is protected,cannot set it again";
            ErrorCodeDefinitions.map[0x0e] = "Tag is unprotected,no need to reset it";
            ErrorCodeDefinitions.map[0x10] = "There is some locked bytes,write fail";
            ErrorCodeDefinitions.map[0x11] = "can not lock it";
            ErrorCodeDefinitions.map[0x12] = "is locked,cannot lock it again";
            ErrorCodeDefinitions.map[0x13] = "Save Fail,Can Use Before Power";
            ErrorCodeDefinitions.map[0x14] = "Cannot adjust";
            ErrorCodeDefinitions.map[0x15] = "Return before Inventory finished";
            ErrorCodeDefinitions.map[0x16] = "Inventory-Scan-Time overflow";
            ErrorCodeDefinitions.map[0x17] = "More Data";
            ErrorCodeDefinitions.map[0x18] = "Reader module MCU is full";
            ErrorCodeDefinitions.map[0x19] = "Not Support Command Or AccessPassword Cannot be Zero";
            ErrorCodeDefinitions.map[0xF9] = "Command execute error";
            ErrorCodeDefinitions.map[0xFA] = "Get Tag,Poor Communication,Inoperable";
            ErrorCodeDefinitions.map[0xFB] = "No Tag Operable";
            ErrorCodeDefinitions.map[0xFC] = "Tag Return ErrorCode";
            ErrorCodeDefinitions.map[0xFD] = "Command length wrong";
            ErrorCodeDefinitions.map[0xFE] = "Illegal command";
            ErrorCodeDefinitions.map[0xFF] = "Parameter Error";
            ErrorCodeDefinitions.map[0x30] = "Communication error";
            ErrorCodeDefinitions.map[0x31] = "CRC checksummat error";
            ErrorCodeDefinitions.map[0x32] = "Return data length error";
            ErrorCodeDefinitions.map[0x33] = "Communication busy";
            ErrorCodeDefinitions.map[0x34] = "Busy,command is being executed";
            ErrorCodeDefinitions.map[0x35] = "ComPort Opened";
            ErrorCodeDefinitions.map[0x36] = "ComPort Closed";
            ErrorCodeDefinitions.map[0x37] = "Invalid Handle";
            ErrorCodeDefinitions.map[0x38] = "Invalid Port";
            ErrorCodeDefinitions.map[0xEE] = "Return command error";
            ErrorCodeDefinitions.map[0x00] = "Other error";
            ErrorCodeDefinitions.map[0x03] = "Memory out or pc not support";
            ErrorCodeDefinitions.map[0x04] = "Memory Locked and unwritable";
            ErrorCodeDefinitions.map[0x0b] = "No Power,memory write operation cannot be executed";
            ErrorCodeDefinitions.map[0x0f] = "Not Special Error,tag not support special errorcode";
            //ErrorCodeDefinitions.map["InventoryReturnEarly_G2"] = "Return before Inventory finished";
            //ErrorCodeDefinitions.map["InventoryTimeOut_G2"] = "the Inventory-scan-time overflow";
            //ErrorCodeDefinitions.map["InventoryMoreData_G2"] = "More Data";
            //ErrorCodeDefinitions.map["ReadermoduleMCUFull_G2"] = "Reader module MCU is Full";
            //ErrorCodeDefinitions.map["AccessPasswordError"] = "Access password error";
            //ErrorCodeDefinitions.map["DestroyPasswordError"] = "Destroy password error";
            //ErrorCodeDefinitions.map["DestroyPasswordCannotZero"] = "Destroy password error cann’t be Zero";
            //ErrorCodeDefinitions.map["TagNotSupportCMD"] = "Tag Not Support the command";
            //ErrorCodeDefinitions.map["AccessPasswordCannotZero"] = "Use the commmand,Access Password Cann’t be Zero";
            //ErrorCodeDefinitions.map["TagProtectedCannotSetAgain"] = "Tag is protected,cannot set it again";
            //ErrorCodeDefinitions.map["TagNoProtectedDonotNeedUnlock"] = "Tag is unprotected,no need to reset it";
            //ErrorCodeDefinitions.map["ByteLockedWriteFail"] = "There is some locked bytes,write fail";
            //ErrorCodeDefinitions.map["CannotLock"] = "can not lock it";
            //ErrorCodeDefinitions.map["LockedCannotLockAgain"] = "is locked,cannot lock it again";
            //ErrorCodeDefinitions.map["ParameterSaveFailCanUseBeforeNoPower"] = "Save Fail,Can Use Before Power";
            //ErrorCodeDefinitions.map["CannotAdjust"] = "Cannot adjust";
            //ErrorCodeDefinitions.map["InventoryReturnEarly_6B"] = "Return before Inventory finished";
            //ErrorCodeDefinitions.map["InventoryTimeOut_6B"] = "Inventory-Scan-Time overflow ";
            //ErrorCodeDefinitions.map["InventoryMoreData_6B"] = "More Data";
            //ErrorCodeDefinitions.map["ReadermoduleMCUFull_6B"] = "Reader module MCU is full";
            //ErrorCodeDefinitions.map["NotSupportCMDOrAccessPasswordCannotZero"] = "Not Support Command Or AccessPassword Cannot be Zero";
            //ErrorCodeDefinitions.map["CMDExecuteErr"] = "Command execute error";
            //ErrorCodeDefinitions.map["GetTagPoorCommunicationCannotOperation"] = "Get Tag,Poor Communication,Inoperable";
            //ErrorCodeDefinitions.map["NoTagOperation"] = "No Tag Operable";
            //ErrorCodeDefinitions.map["TagReturnErrorCode"] = "Tag Return ErrorCode";
            //ErrorCodeDefinitions.map["CMDLengthWrong"] = "Command length wrong";
            //ErrorCodeDefinitions.map["IllegalCMD"] = "Illegal command";
            //ErrorCodeDefinitions.map["ParameterError"] = "Parameter Error";
            //ErrorCodeDefinitions.map["CommunicationErr"] = "Communication error";
            //ErrorCodeDefinitions.map["RetCRCErr"] = "CRC checksummat error";
            //ErrorCodeDefinitions.map["RetDataErr"] = "Return data length error";
            //ErrorCodeDefinitions.map["CommunicationBusy"] = "Communication busy";
            //ErrorCodeDefinitions.map["ExecuteCmdBusy"] = "Busy,command is being executed";
            //ErrorCodeDefinitions.map["ComPortOpened"] = "ComPort Opened";
            //ErrorCodeDefinitions.map["ComPortClose"] = "ComPort Closed";
            //ErrorCodeDefinitions.map["InvalidHandle"] = "Invalid Handle";
            //ErrorCodeDefinitions.map["InvalidPort"] = "Invalid Port ";
            //ErrorCodeDefinitions.map["RecmdErr"] = "Return command error";
            //ErrorCodeDefinitions.map["OtherError"] = "Other error";
            //ErrorCodeDefinitions.map["MemoryOutPcNotSupport"] = "Memory out or pc not support";
            //ErrorCodeDefinitions.map["MemoryLocked"] = "Memory Locked and unwritable";
            //ErrorCodeDefinitions.map["NoPower"] = "No Power,memory write operation cannot be executed";
            //ErrorCodeDefinitions.map["NotSpecialError"] = "Not Special Error,tag not support special errorcode";
        }
        public static String get(int key)
        {
            String value;
            if (map.TryGetValue(key, out value)) return value;
            else return "Unknown";
        }
    }
}
