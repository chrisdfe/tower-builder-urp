async function main() {
  console.log("Hello");
  let result = "hi";

  const input = await readFromStdin();

}

async function readFromStdin() {
  let result = "";
  for await (const chunk of process.stdin) {
    result += chunk;
  }
  return result;
}

main();