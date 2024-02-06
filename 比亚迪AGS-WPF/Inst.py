# 添加三个方法 分别时直接打印，返回字符串，返回列表
import time


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
    # 返回测试结果list， 包含测试名称 参数 值 结果
    data = [{"test_name": "print_hello", "test_param": "", "test_value": "", "test_result": "PASS"},
            {"test_name": "return_hello", "test_param": "", "test_value": "", "test_result": "PASS"},
            {"test_name": "return_hello_list", "test_param": "", "test_value": "", "test_result": "PASS"},
            {"test_name": "print_hello_name", "test_param": "world", "test_value": "hello world",
             "test_result": "PASS"}]

    return format_data(data)


def format_data(data):
    result = []
    for i in data:
        result.append({"Name": i["test_name"], "Upper": 1, "Lower":-1,"Value": i["test_value"],
                       "Result": i["test_result"],"Unit":"mm"})
    return result


# 
# public partial class TestItem:ObservableObject
# {
# 
# private string? _name;
# 
# private double? _lower;
# 
# private double? _upper;
# [ObservableProperty]
# private string? _value;
# [ObservableProperty]
# private string? _result;
# [ObservableProperty]
# private string? _unit;
# }
