import pyvisa as visa
import logging
import requests

logging.basicConfig(level=logging.DEBUG, filemode='w', filename="./logs/LC.log",
                    format='%(asctime)s - %(levelname)s - %(message)s', encoding='utf-8')
# e4890_resouce= '''TCPIP0::192.168.1.14::inst0::INSTR'''
e4890_resouce = '''USB0::0x2A8D::0x2F01::MY46624897::INSTR'''


def connect_device(resource):
    # 创建一个资源管理器
    rm = visa.ResourceManager()
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


# 校准
def calibration(device, mode):
    # mode 选择开路校准和短路校准
    if mode == 'OPEN':
        device.query(':CORR:OPEN')
    elif mode == 'SHORT':
        device.query(':CORR:SHOR')
    else:
        logging.debug("校准模式错误")


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


def config_ls(device):
    # 设置测量功能为电感和串联电阻
    device.write(':FUNC:IMP:TYPE LSRS')
    # 打开自动测量范围
    device.write(':FUNC:IMP:RANG:AUTO ON')
    # 设置测量范围为100欧姆
    device.write(':FUNCtion:IMPedance:RANGe 100;')
    # 设置频率10k
    device.write(':FREQ 10000;')
    # 设置电压
    device.write(':VOLT 1;')
    # 设置电流10mA
    device.write(':CURR 10;')
    # 设置速度和分辨率
    device.write(':APERture MED,1')


def config_cap(device):
    # 设置测量功能为电容和并联电阻
    device.write(':FUNC:IMP:TYPE CPD')
    # 打开自动测量范围
    device.write(':FUNC:IMP:RANG:AUTO ON')
    # 设置测量范围为100欧姆
    device.write(':FUNCtion:IMPedance:RANGe 100;')
    # 设置频率1khz
    device.write(':FREQ 1000;')
    # 设置电压
    device.write(':VOLT 1;')
    # 设置速度和分辨率
    device.write(':APERture MED,1')


def measure(device):
    # 开始测量
    device.write(':INIT')
    # 等待测量完成
    device.query('*OPC?')
    # 读取测量结果
    result = device.query(':FETC?')
    return result


def main():
    plc_step_addr = "D6080"
    # 清除 D5080/D6081 D6082
    write_plc(plc_step_addr, 0)
    write_plc('D6081', 0)
    write_plc('D6082', 0)
    # 连接设备
    device = connect_device(e4890_resouce)
    write_plc(plc_step_addr, 0)
    # 测量电感 DC+ - DC+'
    config_ls(device)
    result = measure(device)
    logging.debug(result)
    write_plc(plc_step_addr, 1)

    # 测量电感 DC- - DC-'
    config_ls(device)
    result = measure(device)
    logging.debug(result)
    write_plc(plc_step_addr, 2)

    # 测量电容 DC+ - DC-
    config_cap(device)
    result = measure(device)
    logging.debug(result)
    write_plc(plc_step_addr, 3)

    # 测量电容 DC+ - gnd
    config_cap(device)
    result = measure(device)
    logging.debug(result)
    write_plc(plc_step_addr, 4)

    # 测量电容 DC- - gnd
    config_cap(device)
    result = measure(device)
    logging.debug(result)
    write_plc(plc_step_addr, 5)
    # 通知完成测试
    write_plc(plc_step_addr, 10)
    logging.debug(result)
    # 关闭设备
    close_device(device)


if __name__ == '__main__':
    main()

# :FUNC:IMP LSRS;  
# :FUNC:IMP:RANG:AUTO ON;  OK
# :FUNCtion:IMPedance:RANGe 100;   设置激励电阻

# :CORR:OPEN;  


# :CORR:SHOR;

# :FREQ 1000;  设置频率hz

# :VOLT 1000;  设置电压毫安
# :CURR 1000;


# :APERture MED,1 是指速度和分辨率 :APERture {SHORt|MEDium|LONG}, <numeric>
# ;:DISPlay:LINE?
# DISP:PAGE MEAS; 显示测试页面
# DISP:ENAB ON; 启用显示更新
# :ABORT;:INIT;
# :TRIG;
# :FETCH?
