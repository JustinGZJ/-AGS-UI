import pyvisa as visa
import matplotlib.pyplot as plt
import niswitch
import logging
import time
import requests

logging.basicConfig(level=logging.DEBUG, filemode='w', filename="./logs/znl.log",
                    format='%(asctime)s - %(levelname)s - %(message)s', encoding='utf-8')
znl_resouce = '''TCPIP0::192.168.1.16::inst0::INSTR'''


def connect_device(resource):
    # 创建一个资源管理器
    rm = visa.ResourceManager()
    # 连接到指定的设备
    device = rm.open_resource(znl_resouce)
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
    # 选择设备的通道1
    device.write('INST:SEL CHANNEL1;')
    # 设置设备的功能为S21功率传输
    device.write('SENS1:FUNC "XFR:POW:S21";')
    # 打开设备的显示更新
    device.write('SYST:DISP:UPD ON;')
    # 设置设备的计算格式为幅度
    device.write('CALC1:FORM MAGN;')
    # 设置设备的源功率级别为0dbm
    device.write('SOUR1:POW:LEV 0dbm;')
    # 设置设备的扫描空间为对数
    device.write('SENS1:SWE:SPAC LOG;')
    device.write('SENS1:SWE:TIME:AUTO ON;')
    device.write('SENS1:SWE:POIN 400;')
    device.write('SENS1:BAND:AUTO ON;')
    device.write('SENS1:BAND 1kHz;')
    device.write('DISP:WIND1:TRAC1:Y:SCAL:TOP 0dB;')
    device.write('DISP:WIND1:TRAC1:Y:SCAL:BOTT -120dB;')
    device.write('SENS1:DET:FUNC NORM;')
    # 设置设备的平均模式为点
    device.write('SENS1:AVER:MODE POIN;')
    # 设置设备的平均计数为2
    device.write('SENS1:AVER:COUN 2;')
    # 打开设备的平均功能
    device.write('SENS1:AVER ON;')
    # 设置设备的源功率级别为0dbm
    device.write('SOUR1:POW:LEV 0dbm;')
    # 设置设备的带宽为1kHz
    device.write('SENS1:BAND 1kHz;')
    # 设置设备的频率起始值为10kHz
    device.write('SENS1:FREQ:START 10 kHz;')
    # 设置设备的频率结束值为500000kHz
    device.write('SENS1:FREQ:STOP 500000 kHz;')
    # 清除设备的所有段
    device.write('SENS1:SEGM:CLE;')
    # 定义设备的各个段的参数
    device.write('SENS1:SEGM:DEF1 10kHz,79.433kHz,10,0dBm,AUTO,2,1000 Hz;')
    device.write('SENS1:SEGM:DEF2 100kHz,425.670kHz,10,0dBm,AUTO,2,1000 Hz;')
    device.write('SENS1:SEGM:DEF3 500kHz,933.033kHz,10,0dBm,AUTO,2,1000 Hz;')
    device.write('SENS1:SEGM:DEF4 1000kHz,2687.875kHz,10,0dBm,AUTO,2,1000 Hz;')
    device.write('SENS1:SEGM:DEF5 3000kHz,16543.950kHz,10,0dBm,AUTO,2,1000 Hz;')
    device.write('SENS1:SEGM:DEF6 20000kHz,158865.600kHz,10,0dBm,AUTO,2,1000 Hz;')
    device.write('SENS1:SEGM:DEF7 200000kHz,500000.000kHz,11,0dBm,AUTO,2,1000 Hz;')
    # 设置设备的频率模式为段
    device.write('SENS1:FREQ:MODE SEGM;')


def get_data(device):
    # 打开设备的显示更新
    device.write('SYST:DISP:UPD ON')
    # 关闭设备的错误显示
    # 关闭设备的自动扫描时间
    device.write('SENS1:SWE:TIME:AUTO OFF')
    # 设置设备的扫描时间为1秒
    device.write('SENS1:SWE:TIME 1')
    # 设置设备的计算格式为对数幅度
    device.write('CALC1:FORM MLOG')
    # 设置设备的图形显示范围为10dB
    device.write('CALC1:GDAP:SCO 10')
    # 设置设备的扫描点数为100
    device.write('SWE:POIN 100')
    # 设置设备的损失补偿值为-12dB
    device.write('CORRection:LOSS:OFFSet -12')
    # 设置设备的频率起始值为10000Hz
    # 设置设备的频率起始值为10000Hz
    device.write('FREQuency:STARt 10000')
    # 设置设备的频率结束值为60000000Hz
    device.write('FREQ:STOP 60000000')
    # 打开设备的连续初始化模式
    device.write('INIT:CONT:ALL ON')
    # 设置设备的扫描次数为1
    device.write('SWE:COUN:ALL 1')
    # 设置设备的初始化范围为全部
    device.write('INIT1:SCOP ALL')
    # 触发设备开始初始化，并等待初始化完成
    device.write('INIT1;*OPC')

    # 对于查询命令，我们需要读取返回的数据
    data_format = device.query_binary_values('FORMAT REAL,32;:CALC:DATA:DALL? FDAT')
    trace_data = device.query_binary_values('TRAC:STIM? CH1DATA')

    # 打印接收的数据
    logging.debug(data_format)
    logging.debug(trace_data)
    return data_format, trace_data


def plot_data(data_format, trace_data):
    plt.plot(trace_data, data_format)
    plt.show()


def switch_channel(channel=1, resource='PXI1Slot2'):
    with niswitch.Session(resource) as session:
        session.disconnect_all()
        session.connect(f'CH{channel}A', 'COMA')
        session.connect(f'CH{channel}B', 'COMB')


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


def measure():
    device = connect_device()
    config(device)
    # 测试两个通道的数据,并同时返回数据
    # 定义一个数组，用于存储两个通道的数据
    data = []
    for i in range(1, 3):
        switch_channel(i)
        # 延时0.5秒
        time.sleep(0.5)
        data_format, trace_data = get_data(device)
        data.append((data_format, trace_data))
        plot_data(data_format, trace_data)
    close_device(device)
    return data


def main():
    write_plc("5120", 0)
    write_plc("6121", 0)
    # measure()
    time.sleep(1)
    write_plc("6121", 1)


if __name__ == "__main__":
    main()
