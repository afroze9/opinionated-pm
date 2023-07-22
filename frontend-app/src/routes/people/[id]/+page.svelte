<script lang="ts">
	import { page } from '$app/stores';
	import LayoutPage from '$lib/layouts/LayoutPage.svelte';
	import { writable } from 'svelte/store';
	import { onMount } from 'svelte';
	import { form, field } from 'svelte-forms';
	import { min, required, email as emailValidator } from 'svelte-forms/validators';
	import ApiHelpers from '../../../services/ApiHelpers';
	import { ProgressRadial, toastStore } from '@skeletonlabs/skeleton';
	import peopleApi, { type PersonResponseModel } from '../../../services/PeopleApi';

	let id: number = +$page.params.id;
	let person = writable<PersonResponseModel>();

	const name = field('name', '', [required(), min(5)], {
		validateOnChange: true,
		valid: false
	});
	const email = field('email', '', [required(), emailValidator()], {
		validateOnChange: true,
		valid: false
	});

	const nameForm = form(name);
	const emailForm = form(email);

	onMount(async () => {
		let response = await peopleApi.getPersonById(id);

		if (!ApiHelpers.isErrorReponse(response)) {
			person.set(response);
			name.set(response.name);
			email.set(response.email);
		} else {
			toastStore.trigger({
				message: response.message,
				background: 'variant-filled-error'
			});
		}
	});

	async function updateName() {
		if (!$nameForm.valid) {
			return;
		}

		let response = await peopleApi.updatePerson(id, {
			id: id,
			name: $name.value
		});

		if (!ApiHelpers.isErrorReponse(response)) {
			toastStore.trigger({
				message: 'Name updated successfully',
				background: 'variant-filled-success'
			});
		} else {
			toastStore.trigger({
				message: response.message,
				background: 'variant-filled-error'
			});
		}
	}

	async function updateEmail() {
		if (!$emailForm.valid) {
			return;
		}

		let response = await peopleApi.updatePerson(id, {
			id: id,
			email: $email.value
		});

		if (!ApiHelpers.isErrorReponse(response)) {
			toastStore.trigger({
				message: 'Email updated successfully',
				background: 'variant-filled-success'
			});
		} else {
			toastStore.trigger({
				message: response.message,
				background: 'variant-filled-error'
			});
		}
	}

	$: getValidationClassForName = (checks: string[]): string => {
		console.log($nameForm.errors);
		return checks.some((check) => $nameForm.hasError(check)) ? 'input-error' : '';
	};
	$: getValidationClassForEmail = (checks: string[]): string => {
		console.log($emailForm.errors);
		return checks.some((check) => $emailForm.hasError(check)) ? 'input-error' : '';
	};
</script>

<LayoutPage>
	{#if $person}
		<h3 class="h3">{$person.name}</h3>

		<label class="label">
			<span>Name</span>
			<div class="input-group input-group-divider grid-cols-[1fr_auto]">
				<input
					class={`input ${getValidationClassForName(['name.required', 'name.min'])}`}
					type="text"
					placeholder="Name"
					bind:value={$name.value}
				/>
				<button
					type="button"
					class="btn variant-filled-secondary ml-4"
					disabled={!$nameForm.valid}
					on:click={updateName}>Update</button
				>
			</div>
		</label>

		<label class="label">
			<span>Email</span>
			<div class="input-group input-group-divider grid-cols-[1fr_auto]">
				<input
					class={`input ${getValidationClassForEmail([
						'email.required',
						'email.not_an_email'
					])}`}
					type="text"
					placeholder="Email"
					bind:value={$email.value}
				/>
				<button
					type="button"
					class="btn variant-filled-secondary ml-4"
					disabled={!$emailForm.valid}
					on:click={updateEmail}>Update</button
				>
			</div>
		</label>
		<div class="flex justify-end">
			<a class="btn variant-filled-error" href="/people">Cancel</a>
		</div>
	{:else}
		<div class="grid h-screen place-items-center"><ProgressRadial /></div>
	{/if}
</LayoutPage>
