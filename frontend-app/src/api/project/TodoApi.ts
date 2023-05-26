import { CreateTodoItemRequest, TodoItem, UpdateTodoItemModel } from "../../@types";
import axios from 'axios';
import { getUrl, getAxiosConfig } from "../configs/axiosConfig";
import { ErrorResponse } from "../ErrorResponse";

const createTodo = async (projectId: number, todo: CreateTodoItemRequest, token: string): Promise<TodoItem | ErrorResponse> => {
  const url = getUrl(`/project/${projectId}/todo`);
  const config = getAxiosConfig(token);

  try {
    const response = await axios.post<TodoItem>(url, todo, config);
    return response.data;
  } catch (e) {
    console.error(e);
    return { message: (e as any).toString() };
  }
}

const updateTodo = async (id: number, todo: UpdateTodoItemModel, token: string): Promise<TodoItem | ErrorResponse> => {
  const url = getUrl(`/todo/${id}`);
  const config = getAxiosConfig(token);

  try {
    const response = await axios.put<TodoItem>(url, todo, config);
    return response.data;
  } catch (e) {
    console.error(e);
    return { message: (e as any).toString() };
  }
}

export default { createTodo, updateTodo }
