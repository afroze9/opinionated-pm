import { Hyperlink } from "./@types"

export const CONSTANTS = {
  APP_NAME: 'Project Management'
}

export const ApplicationLinks: Hyperlink[] = [
  {
    name: 'Companies',
    path: '/companies'
  },
  {
    name: 'Projects',
    path: '/projects'
  },
]

export const baseUrl = 'https://localhost:7068/api/v1';
