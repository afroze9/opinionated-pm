import { Protected, useAuth0 } from "@afroze9/solid-auth0";
import { createForm } from "@felte/solid";
import { validator } from "@felte/validator-yup";
import { Container, Flex, Heading, VStack, FormControl, FormLabel, Input, FormErrorMessage, HStack, Button, SimpleOption, SimpleSelect } from "@hope-ui/solid";
import { useNavigate, Link } from "@solidjs/router";
import { Component, For, createResource } from "solid-js";
import { object, string, InferType, number } from "yup";
import { CompanySummaryResponseModel } from "../../@types";
import { ErrorResponse, isErrorReponse } from "../../api/ErrorResponse";
import CompanyApi from "../../api/company/CompanyApi";
import ProjectApi from "../../api/project/ProjectApi";

type FormProps = {
  name: string;
  company: number;
};

const schema = object({
  name: string().min(5).required(),
  company: number().required()
});

const CreateProject: Component = () => {
  const {
    form,
    errors,
    data,
    isValid,
    setFields
  } = createForm<InferType<typeof schema>>({
    extend: validator({ schema }),
    onSubmit: values => {
      saveProject(values);
    },
  });

  const auth0 = useAuth0();
  const navigate = useNavigate();

  const getCompanyList = async (): Promise<CompanySummaryResponseModel[]> => {
    const c = await CompanyApi.getCompanies(await auth0.getToken());
    if (!isErrorReponse(c)) {
      return c as CompanySummaryResponseModel[];
    }
    return [];
  }

  const [companies] = createResource(getCompanyList);

  const saveProject = async (values: FormProps) => {
    let response = await ProjectApi.createProject({
      name: values.name,
      companyId: values.company
    }, await auth0.getToken());
    if (!isErrorReponse(response)) {
      navigate('/projects');
    } else {
      alert((response as ErrorResponse).message);
    }
  }

  return (
    <Container p="$2">
      <Flex>
        <Heading size="xl">
          Projects
        </Heading>
      </Flex>
      <Container mt="$4">
        <VStack
          as='form'
          ref={form}
          spacing="$5"
          alignItems="stretch"
          maxW="$128"
          mx="auto"
        >
          <FormControl required invalid={!!errors("name")}>
            <FormLabel>Name</FormLabel>
            <Input type="text" name="name" placeholder="Project Name" />
            <FormErrorMessage>{errors("name")?.[0]}</FormErrorMessage>
          </FormControl>
          <FormControl required invalid={!!errors("company")}>
            <FormLabel for="company">Company</FormLabel>
            <SimpleSelect
              placeholder="Choose a company"
              onChange={value => setFields("company", value)}
            >
              <For each={companies()}>
                {item => <SimpleOption value={item.id}>{item.name}</SimpleOption>}
              </For>
            </SimpleSelect>
            <FormErrorMessage>{errors("company")?.[0]}</FormErrorMessage>
          </FormControl>
          <HStack justifyContent="flex-end" spacing="$5">
            <Button type="button" colorScheme="danger" as={Link} href='/companies'>
              Cancel
            </Button>
            <Button type="submit" disabled={!isValid()}>
              Create
            </Button>
          </HStack>
        </VStack>
      </Container>
    </Container>
  );
}

export default () => (
  <Protected onRedirecting={<>Loading</>}>
    <CreateProject />
  </Protected>
)
