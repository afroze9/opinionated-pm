import { Protected, useAuth0 } from "@afroze9/solid-auth0";
import { Button, Container, Flex, FormControl, FormErrorMessage, FormLabel, HStack, Heading, Input, VStack, Tag, TagLabel, TagCloseButton, notificationService, createDisclosure, Modal, ModalBody, ModalCloseButton, ModalContent, ModalFooter, ModalHeader, ModalOverlay, Anchor } from "@hope-ui/solid";
import { Link, useNavigate, useParams } from "@solidjs/router";
import { Component, createEffect, createSignal, For } from "solid-js";
import { CompanyResponse } from "../../@types";
import CompanyApi from "../../api/company/CompanyApi";
import { ErrorResponse, isErrorReponse } from "../../api/ErrorResponse";
import { InferType, object, string } from "yup";
import { createForm } from "@felte/solid";
import { validator } from "@felte/validator-yup";
import { ProjectCard } from "../../components/ProjectCard";


type FormProps = {
  name: string;
};

const schema = object({
  name: string().min(5).required(),
});


const CompanyDetails: Component = () => {
  const auth0 = useAuth0();
  const params = useParams();
  const navigate = useNavigate();

  const [company, setCompany] = createSignal<CompanyResponse>({
    id: 0,
    name: '',
    projects: [],
    tags: [],
  });

  const form = createForm<InferType<typeof schema>>({
    extend: validator({ schema }),
    onSubmit: values => {
      saveCompany(values);
    },
  });

  createEffect(async () => {
    try {
      const data = await getCompany(+params.id);
      setCompany(data);
      form.setFields({
        name: data.name,
      })
    } catch (error) {
      console.error(error);
    }
  })

  async function saveCompany(values: FormProps) {
    let updatedCompany = {
      id: +params.id,
      name: values.name
    };

    let response = await CompanyApi.updateCompany(+params.id, updatedCompany, await auth0.getToken());
    if (!isErrorReponse(response)) {
      navigate('/companies');
    } else {
      alert((response as ErrorResponse).message);
    }
  }

  async function getCompany(id: number): Promise<CompanyResponse> {
    const response = await CompanyApi.getCompanyById(id, await auth0.getToken());
    if (!isErrorReponse(response)) {
      let company = response as CompanyResponse;
      return company;
    }

    return {
      id: 0,
      name: '',
      projects: [],
      tags: [],
    }
  }

  async function deleteTag(companyId: number, tagName: string) {
    if (company()?.tags?.length <= 1) {
      notificationService.show({
        title: "Cannot delete tag",
        description: "The company must have at least one tag",
        status: "danger"
      })
      return;
    }

    await CompanyApi.deleteCompanyTag(companyId, tagName, await auth0.getToken());

    setCompany((prev) => {
      return {
        ...prev,
        tags: prev.tags.filter(x => x.name !== tagName)
      }
    });
  }

  return (
    <Container p="$2">
      <Flex>
        <HStack spacing="$4">
          <Heading size="xl">
            {company()?.name}
          </Heading>
          {company()?.tags?.map(tag => (
            <Tag size="md">
              <TagLabel>{tag.name}</TagLabel>
              <TagCloseButton onClick={() => deleteTag(+params.id, tag.name)} />
            </Tag>
          ))}
        </HStack>
      </Flex>
      <Container mt="$4">
        <VStack
          as='form'
          ref={form.form}
          spacing="$5"
          alignItems="stretch"
          maxW="$128"
          mx="auto"
        >
          <FormControl required invalid={!!form.errors("name")}>
            <FormLabel>Name</FormLabel>
            <Input type="text" name="name" placeholder="Company Name" value={company()?.name} />
            <FormErrorMessage>{form.errors("name")?.[0]}</FormErrorMessage>
          </FormControl>
          <HStack justifyContent="flex-end" spacing="$5">
            <Button type="button" colorScheme="danger" as={Link} href='/companies'>
              Cancel
            </Button>
            <Button type="submit" disabled={!form.isValid()}>
              Update
            </Button>
          </HStack>
        </VStack>
      </Container>

      <Flex mt="$4">
        <Heading size="xl">
          Projects
        </Heading>
      </Flex>

      <Container mt="$4">
        <VStack spacing="$4">
          {
            company() && <For each={company().projects}>
              {item => <ProjectCard id={item.id} name={item.name} tasks={item.taskCount} />}
            </For>
          }
        </VStack>
      </Container>

    </Container>
  )
}

export default () => (
  <Protected onRedirecting={<>Loading</>}>
    <CompanyDetails />
  </Protected>
)
