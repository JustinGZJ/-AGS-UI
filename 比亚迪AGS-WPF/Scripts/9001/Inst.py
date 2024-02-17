# 添加三个方法 分别时直接打印，返回字符串，返回列表
import random
import time
import requests


def print_hello():
    print("hello")


def return_hello():
    return "hello"


def return_hello_list():
    return ["hello", "world"]


# 传参 并打印
def print_hello_name(name):
    print("hello", name)
    return "hello" + name


def main():
    print('world')
    # weather = requests.get('https://wttr.in')
    # print(weather.text)
    # time.sleep(5)
    # 返回测试结果list， 包含测试名称 参数 值 结果
    data = {'Name': 'Loss',
            'Result': 'FAIL',
            'TestItems': [{'Name': 10, 'Category': 'Loss-1', 'Lower': 13, 'Upper': 18, 'Value': -89.5250244140625,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 30, 'Category': 'Loss-1', 'Lower': 21, 'Upper': 28, 'Value': -84.05638122558594,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 60, 'Category': 'Loss-1', 'Lower': 28, 'Upper': 36, 'Value': -77.3224868774414,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 100, 'Category': 'Loss-1', 'Lower': 32, 'Upper': 42, 'Value': -75.47200775146484,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 200, 'Category': 'Loss-1', 'Lower': 39, 'Upper': 63, 'Value': -68.30815887451172,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 300, 'Category': 'Loss-1', 'Lower': 39, 'Upper': 60, 'Value': -64.76971435546875,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 350, 'Category': 'Loss-1', 'Lower': 45, 'Upper': 67, 'Value': -63.43001174926758,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 430, 'Category': 'Loss-1', 'Lower': 45, 'Upper': 85, 'Value': -61.896217346191406,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 550, 'Category': 'Loss-1', 'Lower': 53, 'Upper': 85, 'Value': -59.95332717895508,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 600, 'Category': 'Loss-1', 'Lower': 52, 'Upper': 78, 'Value': -59.1474609375,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 800, 'Category': 'Loss-1', 'Lower': 48, 'Upper': 78, 'Value': -56.66948318481445,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 1000, 'Category': 'Loss-1', 'Lower': 45, 'Upper': 79, 'Value': -54.85145568847656,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 2000, 'Category': 'Loss-1', 'Lower': 41, 'Upper': 71, 'Value': -49.490238189697266,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 5000, 'Category': 'Loss-1', 'Lower': 36, 'Upper': 60, 'Value': -44.159156799316406,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 10000, 'Category': 'Loss-1', 'Lower': 33, 'Upper': 50, 'Value': -44.307945251464844,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 30000, 'Category': 'Loss-1', 'Lower': 32, 'Upper': 55, 'Value': -46.66035461425781,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 40000, 'Category': 'Loss-1', 'Lower': 33, 'Upper': 64, 'Value': -44.47362518310547,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 50000, 'Category': 'Loss-1', 'Lower': 42, 'Upper': 64, 'Value': -39.517818450927734,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 60000, 'Category': 'Loss-1', 'Lower': 35, 'Upper': 55, 'Value': -30.93886947631836,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 80000, 'Category': 'Loss-1', 'Lower': 27, 'Upper': 50, 'Value': -31.575767517089844,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 100000, 'Category': 'Loss-1', 'Lower': 20, 'Upper': 46, 'Value': -35.72417068481445,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 10, 'Category': 'Loss-2', 'Lower': 12, 'Upper': 18, 'Value': -91.60234069824219,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 30, 'Category': 'Loss-2', 'Lower': 21, 'Upper': 28, 'Value': -86.92786407470703,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 60, 'Category': 'Loss-2', 'Lower': 27, 'Upper': 36, 'Value': -81.48145294189453,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 100, 'Category': 'Loss-2', 'Lower': 32, 'Upper': 42, 'Value': -76.21326446533203,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 200, 'Category': 'Loss-2', 'Lower': 39, 'Upper': 55, 'Value': -70.50293731689453,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 300, 'Category': 'Loss-2', 'Lower': 44, 'Upper': 63, 'Value': -67.10383605957031,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 350, 'Category': 'Loss-2', 'Lower': 48, 'Upper': 85, 'Value': -66.2340316772461,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 430, 'Category': 'Loss-2', 'Lower': 49, 'Upper': 85, 'Value': -64.02418518066406,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 550, 'Category': 'Loss-2', 'Lower': 49, 'Upper': 85, 'Value': -62.194671630859375,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 600, 'Category': 'Loss-2', 'Lower': 48, 'Upper': 70, 'Value': -61.09150314331055,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 800, 'Category': 'Loss-2', 'Lower': 47, 'Upper': 80, 'Value': -59.04522705078125,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 1000, 'Category': 'Loss-2', 'Lower': 45, 'Upper': 80, 'Value': -57.25946044921875,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 2000, 'Category': 'Loss-2', 'Lower': 40, 'Upper': 65, 'Value': -52.2741813659668,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 5000, 'Category': 'Loss-2', 'Lower': 37, 'Upper': 55, 'Value': -48.38992691040039,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 10000, 'Category': 'Loss-2', 'Lower': 34, 'Upper': 52, 'Value': -50.05853271484375,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 30000, 'Category': 'Loss-2', 'Lower': 30, 'Upper': 50, 'Value': -55.20138931274414,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 40000, 'Category': 'Loss-2', 'Lower': 35, 'Upper': 65, 'Value': -58.02495193481445,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 50000, 'Category': 'Loss-2', 'Lower': 40, 'Upper': 68, 'Value': -43.62300109863281,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 60000, 'Category': 'Loss-2', 'Lower': 35, 'Upper': 70, 'Value': -44.3762092590332,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 80000, 'Category': 'Loss-2', 'Lower': 26, 'Upper': 65, 'Value': -52.37099838256836,
                           'Result': 'FAIL', 'Unit': 'dB'},
                          {'Name': 100000, 'Category': 'Loss-2', 'Lower': 20, 'Upper': 50, 'Value': -42.278133392333984,
                           'Result': 'FAIL', 'Unit': 'dB'}]}
    # 随机修改TesItems中的值和结果
    for item in data['TestItems']:
        # value 赋值随机数
        item['Value'] = random.randint(0, 100)
        # result 根据上下限给出结果
        if item['Value'] < item['Lower'] or item['Value'] > item['Upper']:
            item['Result'] = 'FAIL'
        else:
            item['Result'] = 'PASS'
    return data

if __name__ == '__main__':
    main()
