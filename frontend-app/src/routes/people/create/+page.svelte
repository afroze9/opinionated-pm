<script lang="ts">
	import LayoutPage from '$lib/layouts/LayoutPage.svelte';
	import { field, form, style } from 'svelte-forms';
	import { required, min, email as emailValidator } from 'svelte-forms/validators';
	import ApiHelpers from '../../../services/ApiHelpers';
	import { goto } from '$app/navigation';
	import { toastStore } from '@skeletonlabs/skeleton';
	import peopleApi from '../../../services/PeopleApi';
	import { writable } from 'svelte/store';

	let isCallingApi = writable<boolean>(false);

	const name = field('name', '', [required(), min(5)], {
		validateOnChange: true,
		valid: false
	});
	const email = field('email', '', [required(), emailValidator()], {
		validateOnChange: true,
		valid: false
	});
	const password = field('password', '', [required(), min(8)], {
		validateOnChange: true,
		valid: false
	});

	const personForm = form(name, email, password);

	async function createPerson() {
		isCallingApi.set(true);
		if ($personForm.valid) {
			let response = await peopleApi.createPerson({
				name: $name.value,
				email: $email.value,
				password: $password.value
			});
			if (!ApiHelpers.isErrorReponse(response)) {
				goto('/people');
			} else {
				toastStore.trigger({
					message: response.message,
					background: 'variant-filled-error'
				});
			}
		}
		isCallingApi.set(false);
	}
</script>

<LayoutPage>
	<header class="space-y-4">
		<div class="flex flex-row">
			<h1 class="h1">People</h1>
		</div>
	</header>
	<hr />
	<section class="space-y-4">
		<label class="label">
			<span>Full Name</span>
			<input
				class="input"
				type="text"
				placeholder="Full Name"
				bind:value={$name.value}
				use:style={{ field: name, valid: 'input-success', invalid: 'input-error' }}
			/>
		</label>
		<label class="label">
			<span>Email</span>
			<input
				class="input"
				type="email"
				placeholder="Email"
				bind:value={$email.value}
				use:style={{ field: email, valid: 'input-success', invalid: 'input-error' }}
			/>
		</label>
		<label class="label">
			<span>Password</span>
			<input
				class="input"
				type="password"
				placeholder="Password"
				bind:value={$password.value}
				use:style={{ field: password, valid: 'input-success', invalid: 'input-error' }}
			/>
		</label>
		<div class="flex justify-end">
			<a class="btn variant-filled-error" href="/people">Cancel</a>
			<button
				type="button"
				class="btn variant-filled ml-4"
				on:click={createPerson}
				disabled={!$personForm.valid || $isCallingApi}>Create</button
			>
		</div>
	</section>
</LayoutPage>
