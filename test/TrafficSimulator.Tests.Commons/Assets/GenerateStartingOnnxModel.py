import onnx
import onnx.helper as helper
import numpy as np

# Define input and output tensors
input_tensor = helper.make_tensor_value_info("Inputs", onnx.TensorProto.FLOAT, [None, 8])  # 4 input neurons
output_tensor = helper.make_tensor_value_info("QValues", onnx.TensorProto.FLOAT, [None, 4])  # 4 output neurons

# Create weight tensors (random values for now)
hidden_size = 16  # Number of neurons in hidden layer

# Randomly initialized weights & biases (for demonstration)
weights1 = np.random.randn(8, hidden_size).astype(np.float32)  # 4 input -> 8 hidden
bias1 = np.zeros(hidden_size, dtype=np.float32)
weights2 = np.random.randn(hidden_size, 4).astype(np.float32)  # 8 hidden -> 4 output
bias2 = np.zeros(4, dtype=np.float32)

# Convert weights to ONNX tensors
weight_tensor1 = helper.make_tensor("W1", onnx.TensorProto.FLOAT, weights1.shape, weights1.flatten())
bias_tensor1 = helper.make_tensor("B1", onnx.TensorProto.FLOAT, bias1.shape, bias1.flatten())
weight_tensor2 = helper.make_tensor("W2", onnx.TensorProto.FLOAT, weights2.shape, weights2.flatten())
bias_tensor2 = helper.make_tensor("B2", onnx.TensorProto.FLOAT, bias2.shape, bias2.flatten())

# Create fully connected layers (Gemm = General Matrix Multiplication)
dense1 = helper.make_node("Gemm", inputs=["Inputs", "W1", "B1"], outputs=["Hidden"], alpha=1.0, beta=1.0)
relu = helper.make_node("Relu", inputs=["Hidden"], outputs=["HiddenActivated"])  # Activation function
dense2 = helper.make_node("Gemm", inputs=["HiddenActivated", "W2", "B2"], outputs=["QValues"], alpha=1.0, beta=1.0)

# Define the computational graph
graph = helper.make_graph(
    [dense1, relu, dense2],  # List of operations
    "SimpleDQN",  # Model name
    [input_tensor],  # Inputs
    [output_tensor],  # Outputs
    [weight_tensor1, bias_tensor1, weight_tensor2, bias_tensor2]  # Weights & Biases
)

# Create and save the ONNX model
model = helper.make_model(graph)
onnx.save(model, "current.onnx")
print("ONNX model saved as current.onnx")
