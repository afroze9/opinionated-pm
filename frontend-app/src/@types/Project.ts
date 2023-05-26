import { TodoItem } from "./TodoItem";

export type ProjectResponse = {
  id: number;
  companyId: number;
  name: string;
  todoItems: TodoItem[];
  priority: Priority;
}

export enum Priority {
  Low = 1,
  Medium = 2,
  High = 3,
  Critical = 4,
}

export type ProjectRequest = {
  name: string;
  companyId: number;
}

export type ProjectSummaryResponseModel = {
  id: number;
  companyId: number;
  name: string;
  taskCount: number;
}

export type UpdateProjectRequest = {
  id: number;
  companyId: number;
  name: string;
  priority: Priority;
}
