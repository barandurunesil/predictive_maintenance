# Predictive Maintenance with LightGBM (ML.NET)

This project implements a **predictive maintenance model** using **ML.NET** and the **LightGBM binary classification algorithm**.

The model predicts whether a machine will fail based on sensor and operational data such as temperature, rotational speed, torque, and tool wear.

## Dataset

Dataset: **AI4I 2020 Predictive Maintenance Dataset**  
Source: https://www.kaggle.com/datasets/shivamb/ai4i-2020-predictive-maintenance-dataset

Note: Column names in this repository omit the unit annotations (e.g., `[K]`, `[rpm]`, `[Nm]`) present in the original dataset.

## Requirements

- .NET
- ML.NET

## Model

Algorithm: **LightGBM Binary Classification**

Features used by the model:

- Type (categorical)
- Air temperature
- Process temperature
- Rotational speed
- Torque
- Tool wear

The categorical feature **Type** is encoded using **One-Hot Encoding**.

## Results

- **Accuracy:** ~99%
- **AUC:** 0.97
- **F1-score:** 0.81

## Usage

Run the console application and provide machine parameters when prompted:

- Type (L / M / H)
- Air Temperature
- Process Temperature
- Rotational Speed
- Torque
- Tool wear

The model will output:

- Prediction (Failure / No Failure)
- Failure Probability
