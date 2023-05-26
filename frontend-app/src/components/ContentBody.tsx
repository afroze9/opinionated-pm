import { Component, lazy } from 'solid-js';
import { Flex, Box } from '@hope-ui/solid';
import { Route, Routes } from '@solidjs/router';
import Home from '../pages/Home';

const Companies = lazy(() => import('../pages/companies/Index'));
const CompanyCreate = lazy(() => import('../pages/companies/Create'));
const CompanyDetails = lazy(() => import('../pages/companies/Details'));

const Projects = lazy(() => import('../pages/projects/Index'));
const ProjectCreate = lazy(() => import('../pages/projects/Create'));
const ProjectDetails = lazy(() => import('../pages/projects/Details'));


export const ContentBody: Component = () => {
  return (
    <Flex flex={1} bg="$accent1">
      <Box css={{ width: '100%' }}>
        <Routes>
          <Route path="/" component={Home} />
          <Route path="/companies" component={Companies} />
          <Route path="/companies/create" component={CompanyCreate} />
          <Route path="/companies/:id/details" component={CompanyDetails} />
          <Route path="/projects" component={Projects} />
          <Route path="/projects/create" component={ProjectCreate} />
          <Route path="/projects/:id/details" component={ProjectDetails} />
        </Routes>
      </Box >
    </Flex >
  );
};
