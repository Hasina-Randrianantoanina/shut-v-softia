import functools
import time
def decorator1(func):
    def wrapper(*args, **kwargs):
        print("Decorator 1")
        return func(*args, **kwargs)
    return wrapper


def decorator2(func):
    def wrapper(*args, **kwargs):
        print("Decorator 2")
        return func(*args, **kwargs)
    return wrapper


@decorator1
@decorator2
def say_hello(name):
    print(f"Hello, {name}!")


def time_logger(func):
    @functools.wraps(func)
    def wrapper(*args, **kwargs):
        start_time = time.time()
        result = func(*args, **kwargs)
        end_time = time.time()
        execution_time = end_time - start_time
        print(f"Function '{func.__name__}' took {execution_time:.4f} seconds to execute. and started at {start_time}")
        return result
    return wrapper

# Example usage:


@time_logger
def example_function():
    time.sleep(2)


@time_logger
def add(a, b):
    return a + b


# Test the decorated functions
example_function()
result = add(5, 3)
print(f"Result of add: {result}")




          
#say_hello("Alice")
