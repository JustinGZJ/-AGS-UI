import pyvisa
import logging
import time

import requests

logging.basicConfig(level=logging.DEBUG, filemode='w', filename="./logs/hipot.log",
                    format='%(asctime)s - %(levelname)s - %(message)s', encoding='utf-8')
tos9301_resouce = '''TCPIP0::192.168.1.13::inst0::INSTR'''


def connect_device(resource):
    # 创建一个资源管理器
    rm = pyvisa.ResourceManager()
    # 连接到指定的设备
    device = rm.open_resource(resource)
    # 判断连接是否成功
    if device.query('*IDN?') == '':
        logging.debug("连接失败")
    else:
        logging.debug("连接成功")
    # 返回设备对象
    return device


def close_device(device):
    # 关闭与仪器的连接
    device.close()


def config(device):
    # 发送ROUT:DCW:TERM 101 命令，配置直流耐压测试中的终端 101。
    device.write('ROUT:DCW:TERM 101,OPEN')
    # 发送ROUT:DCW:TERM 102 命令，配置直流耐压测试中的终端 102。
    device.write('ROUT:DCW:TERM 102,OPEN')
    # 发送ROUT:DCW:TERM 103 命令，配置直流耐压测试中的终端 103。
    device.write('ROUT:DCW:TERM 103,OPEN')
    # 发送ROUT:DCW:TERM 104 命令，配置直流耐压测试中的终端 104。
    device.write('ROUT:DCW:TERM 104,OPEN')
    # 发送SYST:CONF:PHOL 2 命令，配置系统的保持时间。
    device.write('SYST:CONF:PHOL 2')
    # 发送SYST:COMM:RLST REM 命令，配置系统的远程通信模式。
    # device.write('SYST:COMM:RLST REM')
    # 发送FUNC DCW 命令，设置功能为直流耐压测试。
    device.write('FUNC DCW')
    # 发送DCW:VOLT 1000V 命令，设置直流耐压测试的电压为 1000V。
    device.write('DCW:VOLT 1000V')
    # 发送DCW:VOLT:PROT 5000V 命令，设置直流耐压测试的电压保护为 5000V。
    device.write('DCW:VOLT:PROT 5000V')
    # 发送DCW:VOLT:SWE:TIM 1 命令，设置直流耐压测试的电压上升时间为 1s。
    device.write('DCW:VOLT:SWE:TIM 2')
    # 发送DCW:VOLT:TIM:STAT ON 命令，打开直流耐压测试的电压上升时间。
    device.write('DCW:VOLT:TIM:STAT ON')
    # 发送DCW:VOLT:TIM 2S 命令，设置直流耐压测试的电压上升时间为 2s。
    device.write('DCW:VOLT:TIM 2S')
    # 发送SENS:DCW:JUDG 0.001000 命令，设置直流耐压测试的判定电流为 1mA。
    device.write('SENS:DCW:JUDG 0.001000')
    # 发送SENS:DCW:JUDG:LOW:STAT ON 命令，打开直流耐压测试的低电流判定。
    device.write('SENS:DCW:JUDG:LOW:STAT ON')
    # 发送SENS:DCW:JUDG:LOW 0.000000UA 命令，设置直流耐压测试的低电流判定为 0uA。
    device.write('SENS:DCW:JUDG:LOW 0.000000UA')
    # 发送ROUT:DCW:TERM:CCH OFF 命令，关闭直流耐压测试的通道切换。
    # device.write('ROUT:DCW:TERM:CCH OFF')
    # 发送TRIG:TEST:SOUR IMM 命令，设置直流耐压测试的触发源为立即触发。
    device.write('TRIG:TEST:SOUR IMM')


def check_protect(device):
    protect_condition = device.query('STATus:OPERation:PROTecting:CONDition?').strip()
    if protect_condition == '0':
        logging.debug("没有保护条件")
    else:
        logging.debug("有保护条件")
    return protect_condition == "0"


def measure(device, func, TERM1, TERM2, TERM3, TERM4):
    device.write("*RST;*CLS;")
    device.write(f'FUNC {func}')
    # 发送ROUT:DCW:TERM 101,HIGH 命令，配置直流耐压测试终端 101 的高电平。
    device.write(f'ROUT:{func}:TERM 101,{TERM1}')
    # 发送ROUT:DCW:TERM? 101 命令，查询直流耐压测试终端 101 的配置。
    logging.debug(device.query(f'ROUT:{func}:TERM? 101'))
    # 发送ROUT:DCW:TERM 102,LOW 命令，配置直流耐压测试终端 102 的低电平。
    device.write(f'ROUT:{func}:TERM 102,{TERM2}')
    # 发送ROUT:DCW:TERM? 102 命令，查询直流耐压测试终端 102 的配置。
    device.query(f'ROUT:{func}:TERM? 102')
    # 发送ROUT:DCW:TERM 103,OPEN 命令，配置直流耐压测试终端 103 的开路。
    device.write(f'ROUT:{func}:TERM 103,{TERM3}')
    # 发送ROUT:DCW:TERM? 103 命令，查询直流耐压测试终端 103 的配置。
    device.query(f'ROUT:{func}:TERM? 103')
    # 发送ROUT:DCW:TERM 104,OPEN 命令，配置直流耐压测试终端 104 的开路。
    device.write(f'ROUT:{func}:TERM 104,{TERM4}')
    # 发送ROUT:DCW:TERM? 104 命令，查询直流耐压测试终端 104 的配置。
    device.query(f'ROUT:{func}:TERM? 104')
    # 发送STOP 命令，停止当前操作。
    # device.write('STOP')
    # 发送INIT:TEST 命令，初始化测试操作。
    device.write('INIT:TEST;*WAI;')
    # time.sleep(2)
    logging.debug(device.query('*OPC?'))
    time.sleep(3)
    # 发送RES? 命令，查询测试结果。
    result = device.query('RES?')
    # result=device.query("READ?")
    logging.debug(result)
    return result


def main():
    write_plc("D5050", 0)
    write_plc("D6050", 0)
    device = connect_device(tos9301_resouce)
    config(device)
    result = []
    setp1 = measure(device, 'DCW', 'HIGH', 'LOW', 'OPEN', 'OPEN')
    setp2 = measure(device, 'DCW', 'HIGH', 'HIGH', 'LOW', 'OPEN')
    result.append(setp1)
    result.append(setp2)
    close_device(device)
    write_plc("D6050", 1)


def write_plc(address, value):
    # 写入plc
    url = f'http://127.0.0.1:1880/plc/set'
    data = {'address': address, 'value': value}
    response = requests.post(url=url, data=data)
    if response.status_code == 200:
        logging.debug(f"{address}:{value}写入成功")
        return True
    else:
        logging.debug(f"{address}:{value}写入失败")
        return False


if __name__ == '__main__':
    main()

# STATus:OPERation:PROTecting:CONDition?
# +0
# aSTOP
# ROUT:DCW:TERM 101,OPEN
# ROUT:DCW:TERM 102,OPEN
# ROUT:DCW:TERM 103,OPEN
# ROUT:DCW:TERM 104,OPEN
# SYST:CONF:PHOL 2
# SYST:COMM:RLST REM
# FUNC DCW
# DCW:VOLT 50V
# DCW:VOLT:PROT 5000V
# DCW:VOLT:SWE:TIM 1
# DCW:VOLT:TIM:STAT ON
# DCW:VOLT:TIM 2S
# SENS:DCW:JUDG 0.001000
# SENS:DCW:JUDG:LOW:STAT ON
# SENS:DCW:JUDG:LOW 0.000000UA
# ROUT:DCW:TERM:CCH OFF
# TRIG:TEST:SOUR IMM

# ROUT:DCW:TERM 101,HIGH
# ROUT:DCW:TERM? 101
# HIGH
# ROUT:DCW:TERM 102,LOW
# ROUT:DCW:TERM? 102
# LOW
# ROUT:DCW:TERM 103,OPEN
# ROUT:DCW:TERM? 103
# OPEN
# ROUT:DCW:TERM 104,OPEN
# ROUT:DCW:TERM? 104
# OPEN
# STOP

# INIT:TEST
# RES?
# +3,+1,DCW,+2024,+1,+28,+12,+42,+6,+4.85010E+01,+1.91900E-06,+2.52640E+07,+2.00000E+00,PASS

# ABORT:TEST
# aSTOP
# ROUT:DCW:TERM 101,OPEN
# ROUT:DCW:TERM 102,OPEN
# ROUT:DCW:TERM 103,OPEN
# ROUT:DCW:TERM 104,OPEN
# SYST:CONF:PHOL 2
# SYST:COMM:RLST REM
# FUNC DCW
# DCW:VOLT 50V
# DCW:VOLT:PROT 5000V
# DCW:VOLT:SWE:TIM 1
# DCW:VOLT:TIM:STAT ON
# DCW:VOLT:TIM 2S
# SENS:DCW:JUDG 0.001000
# SENS:DCW:JUDG:LOW:STAT ON
# SENS:DCW:JUDG:LOW 0.000000UA
# ROUT:DCW:TERM:CCH OFF
# TRIG:TEST:SOUR IMM
# ROUT:DCW:TERM 101,HIGH
# ROUT:DCW:TERM? 101
# HIGH
# ROUT:DCW:TERM 102,HIGH
# ROUT:DCW:TERM? 102
# HIGH
# ROUT:DCW:TERM 103,LOW
# ROUT:DCW:TERM? 103
# LOW
# ROUT:DCW:TERM 104,OPEN
# ROUT:DCW:TERM? 104
# OPEN
# OPEN
# STOP

# INIT:TEST
# RES?
# +4,+1,DCW,+2024,+1,+28,+12,+42,+16,+4.85790E+01,+1.59100E-06,+3.05210E+07,+2.00000E+00,PASS
# ABORT:TEST


# STOP - 停止当前操作。
# STATus:OPERation:PROTecting:CONDition? - 查询保护条件的操作状态。
# +0 - 未提供足够的上下文来解释这个响应，可能与查询状态有关。
# ROUT:DCW:TERM 101 - 配置直流耐压测试中的终端 101。
# SYST:CONF:PHOL 2 - 配置系统的保持时间。
# SYST:COMM:RLST REM - 配置系统的远程通信模式。
# FUNC DCW - 设置功能为直流耐压测试。
# DCW:VOLT 0.1V - 设置直流耐压测试的电压为 0.1V。
# ROUT:DCW:TERM? 101 - 查询直流耐压测试终端 101 的配置。
# OPEN - 执行开路测试操作。
# INIT:TEST - 初始化测试操作。
# RES? - 查询测试结果。
# 3 - 响应测试结果为 3。
# ABORT:TEST - 中止当前测试操作。
